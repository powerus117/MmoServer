using MmoServer.Connection;
using MmoServer.Connection.Domain;

namespace MmoServer.Messages.Handler;

public interface IMessageHandler
{
    Type MessageType { get; }
    ConnectionState AllowedState { get; }

    Task HandleAsync(ClientConnection connection, object message);
}