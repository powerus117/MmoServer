using ProtoBuf;

namespace MmoShared.Messages.Login
{
    [ProtoContract]
    public class LoginResultSync : Message
    {
        public override ushort Id => MessageIds.LoginResultSync;
        
        [ProtoMember(1)]
        public LoginResultCode ResultCode { get; set; }
    }
}