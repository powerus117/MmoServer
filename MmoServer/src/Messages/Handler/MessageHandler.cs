using MmoServer.Connection;
using MmoServer.Connection.Domain;

namespace MmoServer.Messages.Handler;

public abstract class MessageHandler<T> : IMessageHandler
{
    public Type MessageType => typeof(T);

    public virtual ConnectionState AllowedState { get; } = ConnectionState.Authenticated;

    public Task HandleAsync(ClientConnection connection, object message)
    {
        return HandleAsync(connection, (T)message);
    }

    protected abstract Task HandleAsync(ClientConnection connection, T message);
}