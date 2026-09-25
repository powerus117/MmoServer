using System.Collections.Concurrent;

namespace MmoServer.Commands;

public class CommandQueue
{
    private Queue<ICommand> _commandQueue = new();
    private object _commandLock = new();
    
    public void Add(ICommand command)
    {
        lock (_commandLock)
        {
            _commandQueue.Enqueue(command);
        }
    }

    public List<ICommand> Drain()
    {
        List<ICommand> commands;
        lock (_commandLock)
        {
            commands = _commandQueue.ToList();
            _commandQueue.Clear();
        }

        return commands;
    }
}