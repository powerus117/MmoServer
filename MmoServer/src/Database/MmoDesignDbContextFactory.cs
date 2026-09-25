using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MmoServer.Core;

namespace MmoServer.Database;

public class MmoDesignDbContextFactory : IDesignTimeDbContextFactory<MmoDbContext>
{
    public MmoDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var services = new ServiceCollection();

        services.AddMmoDatabase(configuration);

        var provider = services.BuildServiceProvider();

        var factory = provider.GetRequiredService<
            IDbContextFactory<MmoDbContext>>();

        return factory.CreateDbContext();
    }
}