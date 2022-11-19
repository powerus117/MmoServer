using System.Collections.Generic;
using System.Net.Sockets;
using MmoServer.Connection;
using MmoServer.Login;

namespace MmoServer
{
    public class Server
    {
        private List<Player> _players = new List<Player>();
        private PortListener _portListener;

        private LoginService _loginService;
        
        public bool IsRunning { get; private set; }

        public Server()
        {
            _portListener = new PortListener(this);
            _loginService = new LoginService();
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
            Player newPlayer = new(client, this);
            _players.Add(newPlayer);
            newPlayer.Start();
        }
    }
}