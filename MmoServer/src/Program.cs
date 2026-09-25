using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MmoServer.Database;
using MmoServer.Logging;
using MmoServer.Login;
using MmoServer.Messages;
using MmoServer.PlayerCharacters;
using MmoServer.World;

namespace MmoServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMmoDatabase(configuration);
            serviceCollection.AddSingleton<Server>();
            
            serviceCollection.AddSingleton<LoginService>();
            serviceCollection.AddSingleton<PlayerManager.PlayerManager>();
            serviceCollection.AddSingleton<PlayerCharacterService>();
            serviceCollection.AddSingleton<WorldService>();

            serviceCollection.AddSingleton<MessageManager>();
            
            var provider = serviceCollection.BuildServiceProvider();

            if (!await CheckDbConnection(provider))
                return;
            
            var server = provider.GetRequiredService<Server>();
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

        static async Task<bool> CheckDbConnection(ServiceProvider provider)
        {
            try
            {
                MmoLogger.Log("Checking db connection...");
                
                await using var scope = provider.CreateAsyncScope();

                var factory = scope.ServiceProvider
                    .GetRequiredService<IDbContextFactory<MmoDbContext>>();

                await using var db = await factory.CreateDbContextAsync();

                await db.Database.OpenConnectionAsync();
                await db.Database.CloseConnectionAsync();

                Console.WriteLine("Database connection OK.");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Database connection failed:");
                Console.Error.WriteLine(ex);
                return false;
            }
        }
    }
}
