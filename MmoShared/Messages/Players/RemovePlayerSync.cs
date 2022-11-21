using ProtoBuf;

namespace MmoShared.Messages.Players
{
    [ProtoContract]
    public class RemovePlayerSync : Message
    {
        public override MessageId Id => MessageId.RemovePlayerSync;
        
        [ProtoMember(1)]
        public ulong UserId { get; set; }
    }
}