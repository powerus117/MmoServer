using System.Collections.Concurrent;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using MmoServer.Connection;
using MmoServer.PlayerCharacters;
using MmoShared.Messages.Players;

namespace MmoServer
{
    public class Server
    {
        private ConcurrentDictionary<uint, ClientConnection> _connections = new();
        private PortListener _portListener;
        private uint _currentPlayerIndex;

        private readonly PlayerManager.PlayerManager _playerManager;
        private readonly PlayerCharacterService _playerCharacterService;
        private readonly IServiceProvider _serviceProvider;
        
        public bool IsRunning { get; private set; }

        public Server(PlayerManager.PlayerManager playerManager, PlayerCharacterService playerCharacterService, IServiceProvider serviceProvider)
        {
            _playerManager = playerManager;
            _playerCharacterService = playerCharacterService;
            _serviceProvider = serviceProvider;
            _portListener = new PortListener(this);
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

        public void AddConnection(TcpClient client)
        {
            _currentPlayerIndex++;
            ClientConnection connection = ActivatorUtilities.CreateInstance<ClientConnection>(
                _serviceProvider,
                client,
                _currentPlayerIndex);
            _connections[_currentPlayerIndex] = connection;
            connection.Start();
        }
        
        public void ConnectionQuit(ClientConnection connection)
        {
            var player = connection.Player;
            if (player != null)
            {
                _playerManager.RemovePlayer(player);
                
                var snapshot = player.Data.CreateSnapshot();
                
                _connections.Remove(connection.ConnectionIndex, out _);

                _ = _playerCharacterService.SavePlayer(snapshot);
            
                _playerManager.BroadcastMessage(new RemovePlayerSync()
                {
                    PlayerId = player.Data.Id
                });
            }
            
            connection.Close();
        }
    }
}