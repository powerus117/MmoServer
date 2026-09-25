using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MmoServer.Connection;
using MmoServer.Database;
using MmoServer.Database.Entities;
using MmoShared.Messages.Login;
using MmoShared.Messages.Login.Register;
using User = MmoServer.Database.Entities.User;

namespace MmoServer.Login
{
    public class LoginService
    {
        private const int MinimumPasswordLength = 3;
        private const int MaximumPasswordLength = 20;
        private const int MinimumUsernameLength = 3;
        private const int MaximumUsernameLength = 12;
        
        private readonly IDbContextFactory<MmoDbContext> _dbContextFactory;
        
        public LoginService(IDbContextFactory<MmoDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        
        public async Task<(LoginResultCode resultCode, User? user)> Login(string username, string password)
        {
            if (username == null || username.Length is < MinimumUsernameLength or > MaximumUsernameLength)
                return (LoginResultCode.InvalidCredentials, null);
            
            if (password == null || password.Length is < MinimumPasswordLength or > MaximumPasswordLength)
                return (LoginResultCode.InvalidCredentials, null);

            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var foundUser = await db.Users.FirstOrDefaultAsync(user => user.Username == username);

            if (foundUser == null)
                return (LoginResultCode.InvalidCredentials, null);
            
            string passwordHash = GetPasswordHash(password, foundUser.Salt);

            if (!foundUser.PasswordHash.Equals(passwordHash))
                return (LoginResultCode.InvalidCredentials, null);

            return (LoginResultCode.Success, foundUser);
        }
        
        public async Task<(RegisterResultCode resultCode, User? user)> Register(string username, string password)
        {
            if (username == null || username.Length is < MinimumUsernameLength or > MaximumPasswordLength)
                return (RegisterResultCode.InvalidUsername, null);
            
            if (password == null || password.Length is < MinimumPasswordLength or > MaximumPasswordLength)
                return (RegisterResultCode.InvalidPassword, null);
            
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var foundUser = await db.Users.AnyAsync(user => user.Username == username);
            
            if (foundUser)
                return (RegisterResultCode.UsernameExists, null);
            
            string salt = GenerateSalt();
            string passwordHash = GetPasswordHash(password, salt);

            var newUser = new User()
            {
                Username = username,
                PasswordHash = passwordHash,
                Salt = salt
            };

            // For now, we instantly create a character for the player with the same name
            var newCharacter = new PlayerCharacter()
            {
                CharacterName = username,
                User = newUser
            };

            try
            {
                db.Users.Add(newUser);
                
                db.PlayerCharacters.Add(newCharacter);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (RegisterResultCode.DatabaseError, null);
            }
            
            return (RegisterResultCode.Success, newUser);
        }
        
        private string GetPasswordHash(string password, string salt)
        {
            using SHA256 hashingAlgorithm = SHA256.Create();
            var bytes = hashingAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            return Convert.ToBase64String(bytes);
        }

        private string GenerateSalt()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}