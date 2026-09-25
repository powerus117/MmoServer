using MmoServer.Connection;
using MmoServer.Messages.Handler;
using MmoShared.Messages.Core;

namespace MmoServer.Handlers;

public class QuitNotifyHandler : MessageHandler<QuitNotify>
{
    private readonly ConnectionManager _connectionManager;
    
    public QuitNotifyHandler(ConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    protected override async Task HandleAsync(ClientConnection connection, QuitNotify message)
    {
        await _connectionManager.Disconnect(connection);
    }
}