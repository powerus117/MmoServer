using MmoServer.Connection;
using MmoServer.Messages.Handler;
using MmoShared.Messages.Players.Movement;

namespace MmoServer.World.Handlers;

public class PlayerMoveHandler : MessageHandler<PlayerMoveNotify>
{
    protected override Task HandleAsync(ClientConnection connection, PlayerMoveNotify message)
    {
        connection.Player!.MoveToPosition(message.Position);
        return Task.CompletedTask;
    }
}