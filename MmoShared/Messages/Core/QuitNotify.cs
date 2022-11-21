using ProtoBuf;

namespace MmoShared.Messages.Core
{
    [ProtoContract]
    public class QuitNotify : Message
    {
        public override MessageId Id => MessageId.QuitNotify;
    }
}