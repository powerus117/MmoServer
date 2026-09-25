using System.Diagnostics;
using MmoServer.Core;
using MmoServer.Logging;
using MmoServer.Players;
using MmoShared.Messages;
using MmoShared.Messages.Players;

namespace MmoServer.World
{
    public class WorldService
    {
        private const int TICK_INTERVAL_MS = 600;
        
        private readonly PlayerManager _playerManager;

        private readonly Thread _worldThread;
        
        private readonly Queue<Player> _joinQueue = new();
        
        private Stopwatch _tickStopWatch = new();
        private bool _isRunning;
        private long _tick;

        public WorldService(PlayerManager playerManager)
        {
            _playerManager = playerManager;
            _worldThread = new Thread(TickLoop)
            {
                Name = "World",
                IsBackground = true
            };
        }
        
        public void Start()
        {
            _isRunning = true;
            _worldThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            _worldThread.Join();
        }

        private void TickLoop()
        {
            while (_isRunning)
            {
                _tickStopWatch.Restart();

                Tick();
                _tick++;

                if (_tickStopWatch.ElapsedMilliseconds > 20)
                {
                    MmoLogger.Log("Tick took longer than 20ms");
                }
                    
                var remaining = TimeSpan.FromMilliseconds(TICK_INTERVAL_MS) - _tickStopWatch.Elapsed;

                if (remaining > TimeSpan.Zero)
                {
                    Thread.Sleep(remaining);
                }
            }
        }

        private void Tick()
        {
            ProcessNewPlayers();
            ProcessCommands();
            ProcessMovement();
        }

        public void EnqueuePlayer(Player player)
        {
            _joinQueue.Enqueue(player);
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
        
        private void ProcessCommands()
        {
            foreach (var player in _playerManager.Players.Values)
            {
                foreach (var command in player.CommandQueue.Drain())
                {
                    command.Execute(player);
                }
            }
        }
        
        private void ProcessMovement()
        {
            foreach (var player in _playerManager.Players.Values)
            {
                if (player.TargetPosition == null || player.Data.Position == player.TargetPosition)
                    continue;

                Vector2I relative = player.TargetPosition.Value - player.Data.Position;
                Vector2I move = new Vector2I(Math.Sign(relative.x), Math.Sign(relative.y));
                player.MoveToPosition(player.Data.Position + move);
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