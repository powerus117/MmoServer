using MmoServer.Connection;
using MmoServer.Core;
using MmoServer.Players.Domain;
using MmoShared.Messages;
using MmoShared.Messages.Players.Movement;

namespace MmoServer.Players
{
    public class Player
    {
        private readonly ClientConnection _connection;
        private readonly PlayerManager.PlayerManager _playerManager;

        public PlayerData Data { get; private set; }
        
        public Player(ClientConnection connection, PlayerData selectedCharacter, PlayerManager.PlayerManager playerManager)
        {
            _connection = connection;
            Data = selectedCharacter;
            _playerManager = playerManager;
        }

        public void AddMessage(Message message)
        {
            _connection.AddMessage(message);
        }

        public void MoveToPosition(Vector2I position)
        {
            // TODO: Path finding
            Data.Position = position;

            _playerManager.BroadcastMessage(new PlayerMovedSync()
            {
                UserId = Data.Id,
                Position = Data.Position
            });
        }
    }
}