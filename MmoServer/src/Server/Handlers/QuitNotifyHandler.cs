using MmoServer.Connection;
using MmoServer.Messages.Handler;
using MmoShared.Messages.Core;

namespace MmoServer.Handlers;

public class QuitNotifyHandler : MessageHandler<QuitNotify>
{
    private Server _server;
    
    public QuitNotifyHandler(Server server)
    {
        _server = server;
    }

    protected override Task HandleAsync(ClientConnection connection, QuitNotify message)
    {
        _server.ConnectionQuit(connection);
        return Task.CompletedTask;
    }
}