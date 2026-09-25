namespace MmoServer.Logging;

public static class MmoLogger
{
    public static void Log(object? log)
    {
        Console.WriteLine(log);
    }

    public static void Error(object? log)
    {
        Console.Error.WriteLine(log);
    }
}