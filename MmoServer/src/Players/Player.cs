using MmoServer.Commands;
using MmoServer.Connection;
using MmoServer.Core;
using MmoServer.Players.Domain;
using MmoServer.World;
using MmoShared.Messages;
using MmoShared.Messages.Players.Movement;

namespace MmoServer.Players
{
    public class Player
    {
        private readonly ClientConnection _connection;
        
        private readonly WorldService _worldService;

        public PlayerData Data { get; }
        public CommandQueue CommandQueue { get; } = new();

        public Vector2I? TargetPosition { get; private set; }
        
        public Player(WorldService worldService, ClientConnection connection, PlayerData selectedCharacter)
        {
            _worldService = worldService;
            
            _connection = connection;
            Data = selectedCharacter;
        }

        public void AddMessage(Message message)
        {
            _connection.AddMessage(message);
        }

        public void MoveToPosition(Vector2I position)
        {
            // TODO: Path finding
            Data.Position = position;

            _worldService.BroadcastMessage(new PlayerMovedSync()
            {
                UserId = Data.Id,
                Position = Data.Position
            });
        }

        public void SetMovementTarget(Vector2I targetPosition)
        {
            TargetPosition = targetPosition;
        }
    }
}