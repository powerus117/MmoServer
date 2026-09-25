using MmoServer.Players;
using MmoShared.Messages.Players;

namespace MmoServer.World
{
    public class WorldService
    {
        private readonly PlayerManager.PlayerManager _playerManager;
        
        private readonly Queue<Player> _joinQueue = new();

        public WorldService(PlayerManager.PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void EnqueuePlayer(Player player)
        {
            _joinQueue.Enqueue(player);
            
            // TODO: Do this on single thread world tick
            ProcessNewPlayers();
        }

        void ProcessNewPlayers()
        {
            while (_joinQueue.TryDequeue(out var joinedPlayer))
            {
                _playerManager.AddPlayer(joinedPlayer.Data.Id, joinedPlayer);
                SendWorldState(joinedPlayer);
                
                _playerManager.BroadcastMessage(new AddPlayerSync()
                {
                    PlayerDataDto = joinedPlayer.Data.ToDto()
                });
            }
        }

        public void SendWorldState(Player player)
        {
            var playersSnapshot = _playerManager.Players.ToDictionary(
                pair => pair.Key, 
                pair => pair.Value.Data.ToDto());
            
            playersSnapshot.Remove(player.Data.Id);
            
            player.AddMessage(new LoadedSync()
            {
                Players = playersSnapshot
            });
        }
    }
}