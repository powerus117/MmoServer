using MmoServer.Core;
using ProtoBuf;

namespace MmoShared.Messages.Players.Domain
{
    [ProtoContract]
    public class PlayerData
    {
        private const string DefaultColor = "C52424";
        
        [ProtoMember(1)]
        public Vector2I Position { get; set; }

        [ProtoMember(2)]
        public string Color { get; set; } = DefaultColor;
    }
}