using System;
using System.Collections.Generic;
using System.Linq;
using MmoShared.Messages;

namespace MmoServer.Messages
{
    public static class MessageTypeHelper
    {
        public static readonly Dictionary<MessageId, MessageTypeInfo> IdToTypeMap;

        static MessageTypeHelper()
        {
            IdToTypeMap = new Dictionary<MessageId, MessageTypeInfo>();

            IEnumerable<Type> messageTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()).Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(Message)));

            foreach (Type messageType in messageTypes)
            {
                try
                {
                    var messageInstance = Activator.CreateInstance(messageType);
                    if (messageInstance is Message message)
                    {
                        IdToTypeMap.Add(message.Id, new MessageTypeInfo(messageType));
                    }
                    else
                    {
                        Console.WriteLine($"Instance of message type {messageType.Name} is not of type message: " +
                                          messageInstance);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}