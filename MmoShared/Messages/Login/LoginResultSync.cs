using MmoShared.Messages.Login.Domain;
using MmoShared.Messages.Players.Domain;
using ProtoBuf;

namespace MmoShared.Messages.Login
{
    [ProtoContract]
    public class LoginResultSync : Message
    {
        public override MessageId Id => MessageId.LoginResultSync;
        
        [ProtoMember(1)]
        public LoginResultCode ResultCode { get; set; }
        
        [ProtoMember(2)]
        public PlayerDataDto PlayerDataDto { get; set; }
    }
}