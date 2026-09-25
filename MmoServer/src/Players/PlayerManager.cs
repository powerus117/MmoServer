namespace MmoServer.Players;

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
    
    public bool TryGet(long id, out Player? player)
    {
        return _players.TryGetValue(id, out player);
    }
}