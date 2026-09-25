using MmoServer.Players;
using MmoShared.Messages;
using MmoShared.Messages.Players;

namespace MmoServer.World
{
    public class WorldService
    {
        private readonly PlayerManager _playerManager;
        
        private readonly Queue<Player> _joinQueue = new();

        public WorldService(PlayerManager playerManager)
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
                
                BroadcastMessage(new AddPlayerSync()
                {
                    PlayerDataDto = joinedPlayer.Data.ToDto()
                });
            }
        }
        
        public void BroadcastMessage(Message message)
        {
            foreach (var player in _playerManager.Players.Values)
                player.AddMessage(message);
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