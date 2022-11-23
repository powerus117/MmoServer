using System;
using System.Net.Sockets;
using System.Threading;
using Dapper;
using MmoServer.Core;
using MmoServer.Database;
using MmoServer.Users.Domain;
using MmoShared.Messages;
using MmoShared.Messages.Login.Domain;
using MmoShared.Messages.Players;
using MmoShared.Messages.Players.Domain;
using MmoShared.Messages.Players.Movement;
using MySql.Data.MySqlClient;

namespace MmoServer.Users
{
    public class User
    {
        private readonly Thread _thread;
        private readonly Server _server;
        private readonly Connection.Connection _connection;

        public uint PlayerIndex { get; }
        public bool Loaded { get; private set; }
        public UserInfo UserInfo { get; private set; }
        public UserData Data { get; private set; }

        public User(TcpClient client, Server server, uint playerIndex)
        {
            _server = server;
            PlayerIndex = playerIndex;
            _thread = new(PlayerThread);
            _connection = new Connection.Connection(this, client);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Kill()
        {
            _connection.Close();

            SaveData();
        }

        public void AddMessage(Message message)
        {
            _connection.AddMessage(message);
        }
        
        public void LoadData(ulong id, string username, AccountType accountType)
        {
            UserInfo = new UserInfo(id, username, accountType);
            
            using var conn = new MySqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();
            var result = conn.QueryFirstOrDefault("SELECT * FROM userData WHERE UserId = @userId",
                new { userId = id });
            if (result != null)
            {
                Data = new UserData()
                {
                    Position = new Vector2I(result.PositionX, result.PositionY),
                    Color = result.Color
                };
            }
            else
            {
                Data = new UserData()
                {
                    Position = Vector2I.zero
                };
            }
            Loaded = true;
            
            AddMessage(new LoadedSync
            {
                Players = _server.GetPlayers()
            });
            
            _server.BroadcastMessage(new AddPlayerSync()
            {
                PlayerData = new PlayerData()
                {
                    UserInfo = UserInfo,
                    Position = Data.Position,
                    Color = Data.Color
                }
            });
        }

        public void MoveToPosition(Vector2I position)
        {
            // TODO: Path finding
            Data.Position = position;

            _server.BroadcastMessage(new PlayerMovedSync()
            {
                UserId = UserInfo.UserId,
                Position = Data.Position
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

        private void SaveData()
        {
            if (!Loaded)
            {
                return;
            }
            
            using var conn = new MySqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();
            try
            {
                conn.Execute(
                    "INSERT INTO userData (UserId, PositionX, PositionY, Color) VALUES(@userId, @positionX, @positionY, @color) ON DUPLICATE KEY UPDATE PositionX = @positionX, PositionY = @positionY",
                    new
                    {
                        positionX = Data.Position.x,
                        positionY = Data.Position.y,
                        userId = UserInfo.UserId,
                        color = Data.Color
                    });
                
                Console.WriteLine("Saved player ID: " + UserInfo.UserId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save player ID: " + UserInfo.UserId);
                Console.WriteLine(e);
            }
        }
    }
}