using MmoShared.Messages.Players.Domain;
using ProtoBuf;

namespace MmoShared.Messages.Players
{
    [ProtoContract]
    public class AddPlayerSync : Message
    {
        public override MessageId Id => MessageId.AddPlayerSync;
        
        [ProtoMember(1)]
        public PlayerDataDto PlayerDataDto { get; set; }
    }
}