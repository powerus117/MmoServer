using MmoServer.Players;
using MmoShared.Messages;

namespace MmoServer.PlayerManager;

public class PlayerManager
{
    private Dictionary<long, Player> _players = new();
    
    public IReadOnlyDictionary<long, Player> Players => _players;

    public void AddPlayer(long playerId, Player player)
    {
        _players.Add(playerId, player);
    }

    public void RemovePlayer(Player player)
    {
        _players.Remove(player.Data.Id);
    }
    
    public void BroadcastMessage(Message message)
    {
        foreach (var user in Players.Values)
        {
            user.AddMessage(message);
        }
    }
}