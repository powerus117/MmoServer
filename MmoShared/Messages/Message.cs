using Newtonsoft.Json;

namespace MmoShared.Messages
{
    public abstract class Message
    {
        [JsonIgnore]
        public abstract MessageId Id { get; }
    }
}