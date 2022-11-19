using System;

namespace MmoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            Server server = new();

            server.Start();

            Console.WriteLine("Input allowed");

            while (true)
            {
                string? input = Console.ReadLine();

                if (input == null)
                {
                    continue;
                }

                if (input.Equals("stop"))
                {
                    break;
                }
            }
            
            server.Stop();
        }
    }
}
