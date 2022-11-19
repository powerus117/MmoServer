using ProtoBuf;

namespace MmoShared.Messages.Login
{
    [ProtoContract]
    public class LoginNotify : Message
    {
        public override ushort Id => MessageIds.LoginNotify;
        
        [ProtoMember(1)]
        public string Username { get; set; }
        
        [ProtoMember(2)]
        public string Password { get; set; }
    }
}