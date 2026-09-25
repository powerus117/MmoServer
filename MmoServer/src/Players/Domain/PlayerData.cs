using MmoServer.Core;
using MmoServer.Database.Entities;
using MmoShared.Messages.Login.Domain;
using MmoShared.Messages.Players.Domain;

namespace MmoServer.Players.Domain
{
    public class PlayerDataSnapshot
    {
        public long Id { get; set; }
        public Vector2I Position { get; set; }
        
        public void UpdateDbEntity(PlayerCharacter playerCharacter)
        {
            playerCharacter.PositionX = Position.x;
            playerCharacter.PositionY = Position.y;
        }
    }
    
    public class PlayerData
    {
        public long Id { get; private set; }
        public string PlayerName { get; private set; }
        public Vector2I Position { get; set; }
        public AccountType AccountType { get; set; }

        public PlayerData(PlayerCharacter playerCharacter)
        {
            LoadData(playerCharacter);
        }

        public void LoadData(PlayerCharacter playerCharacter)
        {
            Id = playerCharacter.Id;
            PlayerName = playerCharacter.CharacterName;
            Position = new Vector2I(playerCharacter.PositionX, playerCharacter.PositionY);
        }

        public PlayerDataDto ToDto()
        {
            return new PlayerDataDto()
            {
                PlayerId = Id,
                CharacterName = PlayerName,
                Position = Position,
                AccountType = AccountType
            };
        }

        public PlayerDataSnapshot CreateSnapshot()
        {
            return new PlayerDataSnapshot()
            {
                Id = Id,
                Position = Position,
            };
        }
    }
}