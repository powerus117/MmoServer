using System;
using System.Collections.Generic;
using MmoServer.Users;
using MmoShared.Messages;

namespace MmoServer.Messages
{
    public class MessageSubscription<T> : IMessageSubscription
        where T : Message
    {
        private readonly HashSet<Action<User, T>> _handlers = new();

        public void Add(Action<User, T> handler)
        {
            _handlers.Add(handler);
        }
        
        public void Remove(Action<User, T> handler)
        {
            _handlers.Remove(handler);
        }

        public void Invoke(User user, Message message)
        {
            foreach (var handler in _handlers)
            {
                handler?.Invoke(user, (T)message);
            }
        }
    }
}