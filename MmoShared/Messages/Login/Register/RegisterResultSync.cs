using MmoShared.Messages.Login.Domain;
using ProtoBuf;

namespace MmoShared.Messages.Login.Register
{
    [ProtoContract]
    public class RegisterResultSync : Message
    {
        public override MessageId Id => MessageId.RegisterResultSync;
        
        [ProtoMember(1)]
        public RegisterResultCode ResultCode { get; set; }
        
        [ProtoMember(2)]
        public UserInfo UserInfo { get; set; }
    }
}