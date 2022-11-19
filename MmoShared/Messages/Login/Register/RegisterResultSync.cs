using ProtoBuf;

namespace MmoShared.Messages.Login.Register
{
    [ProtoContract]
    public class RegisterResultSync : Message
    {
        public override ushort Id => MessageIds.RegisterResultSync;
        
        [ProtoMember(1)]
        public RegisterResultCode ResultCode { get; set; }
    }
}