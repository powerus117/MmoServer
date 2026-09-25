using MmoServer.Commands;
using MmoServer.Connection;
using MmoServer.Core;
using MmoServer.Messages.Handler;
using MmoShared.Messages.Players.Movement;

namespace MmoServer.Players.Handlers;

public class PlayerMoveHandler : MessageHandler<PlayerMoveNotify>
{
    protected override Task HandleAsync(ClientConnection connection, PlayerMoveNotify message)
    {
        connection.Player!.CommandQueue.Add(new PlayerMoveCommand(message.Position));
        return Task.CompletedTask;
    }
}

public class PlayerMoveCommand : ICommand
{
    private Vector2I TargetPosition { get; }
    
    public PlayerMoveCommand(Vector2I targetPosition)
    {
        TargetPosition = targetPosition;
    }
    
    public void Execute(Player player)
    {
        player.SetMovementTarget(TargetPosition);
    }
}