using MmoServer.Core;
using ProtoBuf;

namespace MmoShared.Messages.Players.Domain
{
    [ProtoContract]
    public class PlayerData
    {
        [ProtoMember(1)]
        public Vector2I Position { get; set; }
    }
}