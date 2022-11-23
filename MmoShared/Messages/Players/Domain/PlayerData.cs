using MmoServer.Core;
using MmoShared.Messages.Login.Domain;
using ProtoBuf;

namespace MmoShared.Messages.Players.Domain
{
    [ProtoContract]
    public class PlayerData
    {
        [ProtoMember(1)]
        public UserInfo UserInfo { get; set; }
        
        [ProtoMember(2)]
        public Vector2I Position { get; set; }

        [ProtoMember(3)]
        public string Color { get; set; }
    }
}