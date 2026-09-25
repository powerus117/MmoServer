using MmoServer.Connection;
using MmoServer.Players.Domain;

namespace MmoServer.Players.Factory;

public interface IPlayerFactory
{
    Player CreatePlayer(PlayerData playerData, ClientConnection connection);
}