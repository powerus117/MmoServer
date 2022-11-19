using System;
using System.Collections.Generic;
using MmoServer.Core;
using MmoShared.Messages;

namespace MmoServer.Messages
{
    public class MessageManager : Singleton<MessageManager>
    {
        private readonly Dictionary<Type, IMessageSubscription> _subscriptions = new Dictionary<Type, IMessageSubscription>();

        public void Subscribe<T>(Action<Player, T> handler)
            where T : Message
        {
            if (!_subscriptions.TryGetValue(typeof(T), out var subscriptionInfo))
            {
                _subscriptions.Add(typeof(T), subscriptionInfo = new MessageSubscription<T>());
            }
            
            ((MessageSubscription<T>)subscriptionInfo).Add(handler);
        }

        public void Unsubscribe<T>(Action<Player, T> handler)
            where T : Message
        {
            if (_subscriptions.TryGetValue(typeof(T), out var subscriptionInfo))
            {
                ((MessageSubscription<T>)subscriptionInfo).Remove(handler);
            }
        }

        public void Send<T>(Player player, T signal)
            where T : Message
        {
            if (_subscriptions.TryGetValue(signal.GetType(), out var subscriptionInfo))
            {
                try
                {
                    subscriptionInfo.Invoke(player, signal);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}