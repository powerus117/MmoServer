namespace MmoServer.Logging;

public static class MmoLogger
{
    public static void Log(string log)
    {
        Console.WriteLine(log);
    }

    public static void Error(string log)
    {
        Console.Error.WriteLine(log);
    }
}