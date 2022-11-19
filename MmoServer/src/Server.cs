using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using MmoServer.Connection;
using MmoServer.Login;
using MmoServer.Users;
using MmoServer.World;
using MmoShared.Messages;
using MmoShared.Messages.Players;
using MmoShared.Messages.Players.Domain;

namespace MmoServer
{
    public class Server
    {
        private object _mutex = new();
        
        private List<User> _players = new();
        private PortListener _portListener;

        private LoginService _loginService;
        private WorldService _worldService;
        
        public bool IsRunning { get; private set; }

        public Server()
        {
            _portListener = new PortListener(this);
            _loginService = new LoginService();
            _worldService = new WorldService();
        }

        public void Start()
        {
            IsRunning = true;
            _portListener.Start();
        }

        public void Stop()
        {
            _portListener.Close();
            IsRunning = false;
        }

        public void CreatePlayer(TcpClient client)
        {
            User newUser = new(client, this);
            lock (_mutex)
            {
                _players.Add(newUser);
            }
            newUser.Start();
        }

        public Dictionary<ulong,PlayerData> GetPlayers()
        {
            lock (_mutex)
            {
                return _players.Where(user => user.Loaded).ToDictionary(user => user.UserInfo.UserId, user => new PlayerData()
                {
                    Position = user.Position
                });
            }
        }

        public void BroadcastMessage(Message message)
        {
            lock (_mutex)
            {
                foreach (var user in _players.Where(user => user.Loaded))
                {
                    user.AddMessage(message);
                }
            }
        }
    }
}