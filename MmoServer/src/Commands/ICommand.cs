using MmoServer.Players;

namespace MmoServer.Commands;

public interface ICommand
{
    void Execute(Player player);
}