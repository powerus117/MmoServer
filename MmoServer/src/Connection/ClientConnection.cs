using System.Collections.Concurrent;
using System.Net.Sockets;
using MmoServer.Connection.Domain;
using MmoServer.Messages;
using MmoServer.Players;
using MmoServer.Players.Domain;
using MmoShared.Messages;
using Newtonsoft.Json;
using ProtoBuf;
using ProtoBuf.Meta;

namespace MmoServer.Connection
{
    public class ClientConnection
    {
        private const int MaxMessagesPerCycle = 5;
        
        private readonly TcpClient _client;
        private readonly NetworkStream _networkStream;
        private readonly BinaryReader _binaryReader;
        private readonly BinaryWriter _binaryWriter;
        
        private readonly Thread _thread;
        
        private readonly ConcurrentQueue<Message> _incomingMessageQueue = new();
        private readonly ConcurrentQueue<Message> _outgoingMessageQueue = new();

        private readonly MessageManager _messageManager;
        private readonly PlayerManager.PlayerManager _playerManager;

        public uint ConnectionIndex { get; }
        
        public Player? Player { get; private set; }

        public bool IsConnected => _client.Connected;
        public ConnectionState State { get; private set; }
        
        public ClientConnection(PlayerManager.PlayerManager playerManager, MessageManager messageManager, TcpClient client, uint connectionIndex)
        {
            _playerManager = playerManager;
            _messageManager = messageManager;
            
            _client = client;
            ConnectionIndex = connectionIndex;
            
            _networkStream = _client.GetStream();
            _binaryReader = new BinaryReader(_networkStream);
            _binaryWriter = new BinaryWriter(_networkStream);
            
            _thread = new(PlayerThread);
        }
        
        public void Start()
        {
            _thread.Start();
        }
                
        private void PlayerThread()
        {
            try
            {
                while (IsConnected)// && _server.IsRunning)
                {
                    ReadMessages();
                    
                    _ = ProcessMessages();
                    
                    SendMessages();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Close()
        {
            Console.WriteLine("Connection closing from: " + _client.Client.RemoteEndPoint);
            _client.Close();
        }

        public void ReadMessages()
        {
            if (!IsConnected)
            {
                return;
            }
            
            for (int i = 0; i < MaxMessagesPerCycle; i++)
            {
                if (_client.Available <= 0)
                {
                    // Nothing to read
                    break;
                }

                try
                {
                    // Read first uint as message id
                    ushort msgId = _binaryReader.ReadUInt16();
                    if (!Enum.IsDefined(typeof(MessageId), msgId))
                    {
                        throw new ArgumentOutOfRangeException("No message ID found for ID: " + msgId);
                    }
                    
                    MessageId messageId = (MessageId)msgId;
                    
                    if (MessageTypeHelper.IdToTypeMap.TryGetValue(messageId, out var messageTypeInfo))
                    {
                        var deserializedMessage = RuntimeTypeModel.Default.DeserializeWithLengthPrefix(_networkStream, null, messageTypeInfo.MessageType,
                            PrefixStyle.Base128, 0);

                        if (deserializedMessage == null)
                        {
                            throw new NullReferenceException();
                        }

                        Message readMessage = Convert.ChangeType((dynamic)deserializedMessage, messageTypeInfo.MessageType);
                        _incomingMessageQueue.Enqueue(readMessage);
                        
                        Console.WriteLine($"Received message {messageId}: {JsonConvert.SerializeObject(readMessage)}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _client.Close();
                }
            }
        }

        public async Task ProcessMessages()
        {
            if (!IsConnected)
            {
                return;
            }

            for (int i = 0; i < MaxMessagesPerCycle && _incomingMessageQueue.TryDequeue(out var message); i++)
            {
                try
                {
                    await _messageManager.DispatchAsync(this, message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void SendMessages()
        {
            if (!IsConnected)
            {
                return;
            }

            for (int i = 0; i < MaxMessagesPerCycle && _outgoingMessageQueue.TryDequeue(out var message); i++)
            {
                try
                {
                    if (MessageTypeHelper.IdToTypeMap.TryGetValue(message.Id, out var messageTypeInfo))
                    {
                        _binaryWriter.Write((ushort)message.Id);

                        RuntimeTypeModel.Default.SerializeWithLengthPrefix(_networkStream, message, message.GetType(),
                            PrefixStyle.Base128, 0);

                        Console.WriteLine($"Sent message: {message.Id}: {JsonConvert.SerializeObject(message)}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _client.Close();
                }
            }
        }

        public void AddMessage(Message message)
        {
            _outgoingMessageQueue.Enqueue(message);
        }

        public void Authenticate(PlayerData playerData)
        {
            Player = new Player(this, playerData, _playerManager);
            State = ConnectionState.Authenticated;
        }
    }
}