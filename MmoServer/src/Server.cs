using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using MmoServer.Connection;
using MmoServer.Login;
using MmoServer.Messages;
using MmoServer.Users;
using MmoServer.World;
using MmoShared.Messages;
using MmoShared.Messages.Core;
using MmoShared.Messages.Players;
using MmoShared.Messages.Players.Domain;

namespace MmoServer
{
    public class Server
    {
        private ConcurrentDictionary<uint, User> _players = new();
        private PortListener _portListener;
        private uint _currentPlayerIndex;

        private LoginService _loginService;
        private WorldService _worldService;
        
        public bool IsRunning { get; private set; }

        public Server()
        {
            _portListener = new PortListener(this);
            _loginService = new LoginService();
            _worldService = new WorldService();
            
            MessageManager.Instance.Subscribe<QuitNotify>(OnQuitNotify);
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
            
            MessageManager.Instance.Unsubscribe<QuitNotify>(OnQuitNotify);
        }

        public void CreatePlayer(TcpClient client)
        {
            _currentPlayerIndex++;
            User newUser = new(client, this, _currentPlayerIndex);
            _players[_currentPlayerIndex] = newUser;
            newUser.Start();
        }

        public Dictionary<ulong,PlayerData> GetPlayers()
        {
            return _players.Values.Where(user => user.Loaded).ToDictionary(user => user.UserInfo.UserId, user => new PlayerData()
            {
                Position = user.Position
            });
        }

        public void BroadcastMessage(Message message)
        {
            foreach (var user in _players.Values.Where(user => user.Loaded))
            {
                user.AddMessage(message);
            }
        }
        
        private void OnQuitNotify(User user, QuitNotify notify)
        {
            user.Kill();

            _players.Remove(user.PlayerIndex, out _);

            if (!user.Loaded)
            {
                return;
            }
            
            BroadcastMessage(new RemovePlayerSync()
            {
                UserId = user.UserInfo.UserId
            });
        }
    }
}