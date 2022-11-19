using System;
using System.Collections.Generic;
using MmoShared.Messages;

namespace MmoServer.Messages
{
    public class MessageSubscription<T> : IMessageSubscription
        where T : Message
    {
        private readonly HashSet<Action<Player, T>> _handlers = new();

        public void Add(Action<Player, T> handler)
        {
            _handlers.Add(handler);
        }
        
        public void Remove(Action<Player, T> handler)
        {
            _handlers.Remove(handler);
        }

        public void Invoke(Player player, Message message)
        {
            foreach (var handler in _handlers)
            {
                handler?.Invoke(player, (T)message);
            }
        }
    }
}