using System;
using System.Net.Sockets;
using System.Threading;
using MmoShared.Messages;

namespace MmoServer
{
    public class Player
    {
        private readonly Thread _thread;
        private readonly Server _server;
        private readonly Connection.Connection _connection;

        public Player(TcpClient client, Server server)
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