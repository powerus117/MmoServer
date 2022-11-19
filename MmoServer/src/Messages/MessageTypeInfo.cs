using System;

namespace MmoServer.Messages
{
    public struct MessageTypeInfo
    {
        public Type MessageType { get; }

        public MessageTypeInfo(Type messageType)
        {
            MessageType = messageType;
        }
    }
}