using System;
using System.Net.Sockets;
using System.Threading;
using MmoServer.Core;
using MmoShared.Messages;
using MmoShared.Messages.Login.Domain;
using MmoShared.Messages.Players;

namespace MmoServer.Users
{
    public class User
    {
        private readonly Thread _thread;
        private readonly Server _server;
        private readonly Connection.Connection _connection;

        public bool Loaded { get; private set; }
        public UserInfo UserInfo { get; private set; }
        public Vector2I Position { get; private set; }

        public User(TcpClient client, Server server)
        {
            _server = server;
            _thread = new(PlayerThread);
            _connection = new Connection.Connection(this, client);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void AddMessage(Message message)
        {
            _connection.AddMessage(message);
        }
        
        public void LoadData(ulong id, string username)
        {
            UserInfo = new UserInfo(id, username);
            
            // Temp, should load from db
            Position = Vector2I.zero;
            Loaded = true;
            
            AddMessage(new LoadedSync
            {
                Players = _server.GetPlayers()
            });
        }

        public void MoveToPosition(Vector2I position)
        {
            // TODO: Path finding and smooth walking
            Position = position;

            _server.BroadcastMessage(new PlayerMovedSync()
            {
                UserId = UserInfo.UserId,
                Position = Position
            });
        }
        
        private void PlayerThread()
        {
            try
            {
                while (_connection.IsConnected && _server.IsRunning)
                {
                    _connection.ReadMessages();
                    
                    _connection.ProcessMessages();
                    
                    _connection.SendMessages();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}