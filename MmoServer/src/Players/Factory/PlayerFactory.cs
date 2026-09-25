using Microsoft.Extensions.DependencyInjection;
using MmoServer.Connection;
using MmoServer.Players.Domain;

namespace MmoServer.Players.Factory;

public class PlayerFactory : IPlayerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PlayerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Player CreatePlayer(PlayerData playerData, ClientConnection connection)
    {
        return ActivatorUtilities.CreateInstance<Player>(
            _serviceProvider,
            connection,
            playerData);
    }
}