using Microsoft.EntityFrameworkCore;
using MmoServer.Database;
using MmoServer.Logging;
using MmoServer.Players.Domain;

namespace MmoServer.PlayerCharacters;

public class PlayerCharacterService
{
    private readonly IDbContextFactory<MmoDbContext> _dbContextFactory;
        
    public PlayerCharacterService(IDbContextFactory<MmoDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<PlayerData[]> LoadCharacters(long userId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        
        var characters = await db.PlayerCharacters
            .Where(character => character.UserId == userId)
            .ToArrayAsync();

        return characters.Select(character => new PlayerData(character)).ToArray();
    }
    
    public async Task<PlayerData> LoadPlayer(long playerId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        var foundPlayerCharacter = await db.PlayerCharacters.FindAsync(playerId);

        if (foundPlayerCharacter == null)
            return null;

        var playerData = new PlayerData(foundPlayerCharacter);
        return playerData;
    }

    public async Task SavePlayer(PlayerDataSnapshot dataSnapshot)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        var foundPlayerCharacter = await db.PlayerCharacters.FindAsync(dataSnapshot.Id);

        if (foundPlayerCharacter == null)
        {
            MmoLogger.Error("Tried to save player that doesn't exist with ID: " + dataSnapshot.Id);
            return;
        }

        try
        {
            dataSnapshot.UpdateDbEntity(foundPlayerCharacter);
            await db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            MmoLogger.Error($"Failed to save character {dataSnapshot.Id} with error: {e}");
        }
    }
}