using System.Buffers.Binary;
using System.Net.Sockets;
using System.Threading.Channels;
using MmoServer.Connection.Domain;
using MmoServer.Logging;
using MmoServer.Messages;
using MmoServer.Players;
using MmoShared.Messages;
using Newtonsoft.Json;
using ProtoBuf;
using ProtoBuf.Meta;

namespace MmoServer.Connection
{
    public class ClientConnection
    {
        private const int MAX_MESSAGE_SIZE = 1024 * 1024;
        
        public event Action<ClientConnection>? ConnectionLost;

        private readonly TcpClient _client;
        private readonly NetworkStream _networkStream;
        private readonly MessageManager _messageManager;

        private readonly Channel<Message> _outgoingMessages = Channel.CreateUnbounded<Message>();

        public bool IsConnected => Volatile.Read(ref _closed) == 0;

        public Player? Player { get; private set; }
        public uint ConnectionIndex { get; }
        public ConnectionState State { get; private set; }
        
        private int _closed;

        public ClientConnection(MessageManager messageManager, TcpClient client, uint connectionIndex)
        {
            _messageManager = messageManager;

            _client = client;
            ConnectionIndex = connectionIndex;

            _networkStream = _client.GetStream();
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(ReadLoopAsync(cancellationToken), SendLoopAsync(cancellationToken));
        }

        private async Task ReadLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && IsConnected)
                {
                    Message message = await ReadMessage(cancellationToken);

                    MmoLogger.Log($"Received message {message.Id}: " + JsonConvert.SerializeObject(message));

                    await _messageManager.DispatchAsync(this, message);
                }
            }
            catch (OperationCanceledException)
            {
                Close();
            }
            catch (IOException)
            {
                Close();
            }
            catch (Exception e)
            {
                MmoLogger.Error(e);
                Close();
            }
        }

        private async Task<Message> ReadMessage(CancellationToken cancellationToken)
        {
            // Read first uint as message ID
            ushort msgId = await ReadUInt16Async(cancellationToken);

            if (!Enum.IsDefined(typeof(MessageId), msgId))
                throw new ArgumentOutOfRangeException(nameof(msgId), $"No message ID found for ID: {msgId}");

            MessageId messageId = (MessageId)msgId;

            if (!MessageTypeHelper.IdToTypeMap.TryGetValue(messageId, out var messageTypeInfo))
                throw new InvalidDataException($"No message type registered for {messageId}");
            
            // Read length of protobuf payload, this allows us to async wait until the payload has arrived before blocking/deserializing
            int length = await ReadVarIntAsync(cancellationToken);
            
            if (length > MAX_MESSAGE_SIZE)
                throw new InvalidDataException("Message is too large, read length: " + length);

            byte[] buffer = new byte[length];

            await ReadExactlyAsync(buffer, cancellationToken);

            using var stream = new MemoryStream(buffer);

            var message = RuntimeTypeModel.Default.Deserialize(stream, null, messageTypeInfo.MessageType);

            if (message is not Message result)
                throw new InvalidDataException($"Failed to deserialize message {messageId}");

            return result;
        }

        private async Task SendLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var message in _outgoingMessages.Reader.ReadAllAsync(cancellationToken))
                {
                    if (MessageTypeHelper.IdToTypeMap.TryGetValue(message.Id, out var messageTypeInfo))
                    {
                        byte[] messageId = new byte[2];
                        BinaryPrimitives.WriteUInt16LittleEndian(messageId, (ushort)message.Id);
                        await _networkStream.WriteAsync(messageId, cancellationToken);
                    
                        // First write the serialized message to a local stream
                        using var stream = new MemoryStream();

                        RuntimeTypeModel.Default.SerializeWithLengthPrefix(
                            stream,
                            message,
                            messageTypeInfo.MessageType,
                            PrefixStyle.Base128,
                            0);

                        // Then send the message async so it never blocks a thread
                        await _networkStream.WriteAsync(stream.GetBuffer().AsMemory(0, (int)stream.Length),
                            cancellationToken);

                        MmoLogger.Log($"Sent message: {message.Id}: " + JsonConvert.SerializeObject(message));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Close();
            }
            catch (IOException)
            {
                Close();
            }
            catch (Exception e)
            {
                MmoLogger.Error(e);
                Close();
            }
        }

        public void AddMessage(Message message)
        {
            if (!IsConnected)
                return;

            _outgoingMessages.Writer.TryWrite(message);
        }

        public void Authenticate(Player player)
        {
            Player = player;
            State = ConnectionState.Authenticated;
        }

        public void Close()
        {
            if (Interlocked.Exchange(ref _closed, 1) != 0)
                return;

            MmoLogger.Log("Connection closing from: " + _client.Client.RemoteEndPoint);
            
            _outgoingMessages.Writer.TryComplete();

            try
            {
                _client.Close();
            }
            catch
            {
                // ignored
            }

            ConnectionLost?.Invoke(this);
        }
        
        private async Task<ushort> ReadUInt16Async(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[2];
            
            await ReadExactlyAsync(buffer, cancellationToken);
            
            return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        }
        
        private async Task<int> ReadVarIntAsync(CancellationToken cancellationToken)
        {
            int result = 0;
            int shift = 0;

            while (true)
            {
                byte value = await ReadByteAsync(cancellationToken);

                result |= (value & 0x7F) << shift;

                // If the last bit is 1, the full size is received
                if ((value & 0x80) == 0)
                    return result;

                shift += 7;

                if (shift >= 32)
                    throw new InvalidDataException("Invalid message length prefix.");
            }
        }
        
        private async Task<byte> ReadByteAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1];

            await ReadExactlyAsync(buffer, cancellationToken);

            return buffer[0];
        }
        
        private async Task ReadExactlyAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            int offset = 0;

            while (offset < buffer.Length)
            {
                int bytesRead = await _networkStream.ReadAsync(
                    buffer.AsMemory(offset, buffer.Length - offset),
                    cancellationToken);

                if (bytesRead == 0)
                    throw new IOException("Connection closed while reading message.");

                offset += bytesRead;
            }
        }
    }
}