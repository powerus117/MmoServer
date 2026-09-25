using Microsoft.EntityFrameworkCore;
using MmoServer.Database.Entities;

namespace MmoServer.Database;

public class MmoDbContext : DbContext
{
    public DbSet<User> Users { get; private set; }
    public DbSet<PlayerCharacter> PlayerCharacters { get; private set; }

    public MmoDbContext(DbContextOptions options) : base(options)
    {
    }
}