using MmoServer.Core;
using ProtoBuf;

namespace MmoShared.Messages.Players
{
    [ProtoContract]
    public class PlayerMoveNotify : Message
    {
        public override MessageId Id => MessageId.PlayerMoveNotify;
        
        [ProtoMember(1)]
        public Vector2I Position { get; set; }
    }
}