using ProtoBuf;

namespace MmoShared.Messages.Login.Register
{
    [ProtoContract]
    public class RegisterNotify : Message
    {
        public override ushort Id => MessageIds.RegisterNotify;
        
        [ProtoMember(1)]
        public string Username { get; set; }
        
        [ProtoMember(2)]
        public string Password { get; set; }
    }
}