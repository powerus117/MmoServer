using MmoServer.Core;
using MmoShared.Messages.Login.Domain;
using ProtoBuf;

namespace MmoShared.Messages.Players.Domain
{
    [ProtoContract]
    public class PlayerDataDto
    {
        [ProtoMember(1)]
        public long PlayerId { get; set; }
        
        [ProtoMember(2)]
        public string CharacterName { get; set; }
        
        [ProtoMember(3)]
        public AccountType AccountType { get; set; }
        
        [ProtoMember(4)]
        public Vector2I Position { get; set; }
    }
}