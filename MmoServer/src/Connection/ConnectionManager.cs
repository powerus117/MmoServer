using System.Collections.Concurrent;
using System.Net.Sockets;
using MmoServer.Connection.Factory;
using MmoServer.Logging;
using MmoServer.PlayerCharacters;
using MmoServer.Players;
using MmoServer.World;
using MmoShared.Messages.Players;

namespace MmoServer.Connection;

public class ConnectionManager
{
    private readonly IClientConnectionFactory _factory;
    private readonly ConcurrentDictionary<uint, ClientConnection> _connections = new();
    private readonly PlayerManager _playerManager;
    private readonly PlayerCharacterService _playerCharacterService;
    private readonly WorldService _worldService;

    private uint _nextConnectionIndex;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ConnectionManager(IClientConnectionFactory factory, PlayerManager playerManager, PlayerCharacterService playerCharacterService, WorldService worldService)
    {
        _factory = factory;
        _playerManager = playerManager;
        _playerCharacterService = playerCharacterService;
        _worldService = worldService;
        
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Add(TcpClient client)
    {
        var index = _nextConnectionIndex++;

        var connection = _factory.Create(client, index);
        connection.ConnectionLost += ConnectionLost;

        _connections.TryAdd(index, connection);

        _ = connection.RunAsync(_cancellationTokenSource.Token);
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async void ConnectionLost(ClientConnection connection)
    {
        try
        {
            await Disconnect(connection);
        }
        catch (Exception e)
        {
            MmoLogger.Error(e);
        }
    }

    public async Task Disconnect(ClientConnection connection)
    {
        if (!_connections.TryRemove(connection.ConnectionIndex, out _))
            return;

        var player = connection.Player;

        if (player != null)
        {
            _playerManager.RemovePlayer(player);

            var snapshot = player.Data.CreateSnapshot();

            await _playerCharacterService.SavePlayer(snapshot);

            _worldService.BroadcastMessage(
                new RemovePlayerSync
                {
                    PlayerId = player.Data.Id
                });
        }

        connection.Close();
        connection.ConnectionLost -= ConnectionLost;
    }
}