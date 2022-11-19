using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using MmoServer.Messages;
using MmoShared.Messages;
using ProtoBuf;
using ProtoBuf.Meta;

namespace MmoServer.Connection
{
    public class Connection
    {
        private const int MaxMessagesPerCycle = 5;
        
        private readonly TcpClient _client;
        private readonly NetworkStream _networkStream;
        private readonly BinaryReader _binaryReader;
        private readonly BinaryWriter _binaryWriter;
        private readonly Player _player;
        
        private readonly ConcurrentQueue<Message> _incomingMessageQueue = new();
        private readonly ConcurrentQueue<Message> _outgoingMessageQueue = new();

        public bool IsConnected => _client.Connected;
        
        public Connection(Player player, TcpClient client)
        {
            _player = player;
            _client = client;
            _networkStream = _client.GetStream();
            _binaryReader = new BinaryReader(_networkStream);
            _binaryWriter = new BinaryWriter(_networkStream);
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
                    Console.WriteLine("Received message ID " + msgId);

                    if (MessageTypeHelper.IdToTypeMap.TryGetValue(msgId, out var messageTypeInfo))
                    {
                        var deserializedMessage = RuntimeTypeModel.Default.DeserializeWithLengthPrefix(_networkStream, null, messageTypeInfo.MessageType,
                            PrefixStyle.Base128, 0);

                        if (deserializedMessage == null)
                        {
                            throw new NullReferenceException();
                        }

                        Message readMessage = Convert.ChangeType((dynamic)deserializedMessage, messageTypeInfo.MessageType);
                        _incomingMessageQueue.Enqueue(readMessage);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _client.Close();
                }
            }
        }

        public void ProcessMessages()
        {
            if (!IsConnected)
            {
                return;
            }

            for (int i = 0; i < MaxMessagesPerCycle && _incomingMessageQueue.TryDequeue(out var message); i++)
            {
                try
                {
                    MessageManager.Instance.Send(_player, message);
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
                        _binaryWriter.Write(message.Id);

                        RuntimeTypeModel.Default.SerializeWithLengthPrefix(_networkStream, message, message.GetType(),
                            PrefixStyle.Base128, 0);

                        Console.WriteLine("Sent: {0}", JsonSerializer.Serialize(message, message.GetType()));
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
    }
}