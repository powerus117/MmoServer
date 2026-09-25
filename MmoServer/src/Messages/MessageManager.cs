using Microsoft.Extensions.DependencyInjection;
using MmoServer.Connection;
using MmoServer.Logging;
using MmoServer.Messages.Handler;
using MmoShared.Messages;

namespace MmoServer.Messages
{
    public class MessageManager
    {
        private readonly Dictionary<Type, IMessageHandler> _handlers = new();

        public MessageManager(IServiceProvider serviceProvider)
        {
            var assembly = typeof(IMessageHandler).Assembly;

            var handlerTypes = assembly
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(IMessageHandler).IsAssignableFrom(t));

            foreach (var handlerType in handlerTypes)
            {
                var handler = (IMessageHandler)ActivatorUtilities.CreateInstance(
                    serviceProvider,
                    handlerType);
                
                _handlers.Add(handler.MessageType, handler);
            }
        }

        public Task DispatchAsync(ClientConnection connection, Message message)
        {
            if (!_handlers.TryGetValue(
                    message.GetType(),
                    out var handler))
            {
                MmoLogger.Log("No message handler found for message type: " + message.GetType());
                return Task.CompletedTask;
            }

            if (handler.AllowedState != connection.State)
            {
                // Ignore
                return Task.CompletedTask;
            }

            return handler.HandleAsync(connection, message);
        }
    }
}