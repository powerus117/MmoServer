namespace MmoServer.Database.Entities;

public class PlayerCharacter
{
    public long Id { get; set; }
    public string CharacterName { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public byte AccountType { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; }
}