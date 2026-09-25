using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MmoServer.Database;

public static class MmoDbContextCreator
{
    public static void AddMmoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var database = configuration.GetSection("Database");

        string host = database["Host"]!;
        string port = database["Port"]!;
        string name = database["Name"]!;
        string username = database["Username"]!;
        string password = database["Password"]!;

        string connectionString = $"Host={host};Port={port};Database={name};Username={username};Password={password}";

        services.AddDbContextFactory<MmoDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.UseSnakeCaseNamingConvention();
        });
    }
}