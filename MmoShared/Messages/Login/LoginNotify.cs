using Newtonsoft.Json;
using ProtoBuf;

namespace MmoShared.Messages.Login
{
    [ProtoContract]
    public class LoginNotify : Message
    {
        public override MessageId Id => MessageId.LoginNotify;
        
        [ProtoMember(1)]
        public string Username { get; set; }
        
        [JsonIgnore]
        [ProtoMember(2)]
        public string Password { get; set; }
    }
}