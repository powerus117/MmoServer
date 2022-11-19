using MmoServer.Core;
using ProtoBuf;

namespace MmoShared.Messages.Players
{
    [ProtoContract]
    public class PlayerMovedSync : Message
    {
        public override MessageId Id => MessageId.PlayerMovedSync;
        
        [ProtoMember(1)]
        public ulong UserId { get; set; }
        
        [ProtoMember(2)]
        public Vector2I Position { get; set; }
    }
}