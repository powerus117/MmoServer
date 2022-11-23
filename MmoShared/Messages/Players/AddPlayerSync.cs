using MmoShared.Messages.Players.Domain;
using ProtoBuf;

namespace MmoShared.Messages.Players
{
    [ProtoContract]
    public class AddPlayerSync : Message
    {
        public override MessageId Id => MessageId.AddPlayerSync;
        
        [ProtoMember(1)]
        public ulong UserId { get; set; }
        
        [ProtoMember(2)]
        public PlayerData PlayerData { get; set; }
    }
}