using System;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using MmoServer.Database;
using MmoServer.Messages;
using MmoShared.Messages.Login;
using MmoShared.Messages.Login.Register;
using MySql.Data.MySqlClient;

namespace MmoServer.Login
{
    public class LoginService
    {
        private const int MinimumPasswordLength = 3;
        private const int MaximumPasswordLength = 20;
        private const int MinimumUsernameLength = 3;
        private const int MaximumUsernameLength = 12;
        
        public LoginService()
        {
            MessageManager.Instance.Subscribe<LoginNotify>(OnLoginNotify);
            MessageManager.Instance.Subscribe<RegisterNotify>(OnRegisterNotify);
        }

        private void OnLoginNotify(Player player, LoginNotify notify)
        {
            var resultCode = Login(notify);
            
            player.AddMessage(new LoginResultSync()
            {
                ResultCode = resultCode
            });
        }
        
        private void OnRegisterNotify(Player player, RegisterNotify notify)
        {
            var resultCode = Register(notify);
            
            player.AddMessage(new RegisterResultSync()
            {
                ResultCode = resultCode
            });
        }
        
        private LoginResultCode Login(LoginNotify notify)
        {
            if (notify.Username == null || notify.Username.Length is < MinimumUsernameLength or > MaximumPasswordLength)
            {
                return LoginResultCode.InvalidCredentials;
            }
            
            if (notify.Password == null || notify.Password.Length is < MinimumPasswordLength or > MaximumPasswordLength)
            {
                return LoginResultCode.InvalidCredentials;
            }
            
            using var conn = new MySqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();
            var result = conn.QueryFirstOrDefault("SELECT * FROM users WHERE UserName = @Username",
                new { username = notify.Username });

            if (result != null)
            {
                string passwordHash = GetPasswordHash(notify.Password, result.Salt);
                
                if (result.Password.Equals(passwordHash))
                {
                    return LoginResultCode.Success;
                }
            }

            return LoginResultCode.InvalidCredentials;
        }
        
        private RegisterResultCode Register(RegisterNotify notify)
        {
            if (notify.Username == null || notify.Username.Length is < MinimumUsernameLength or > MaximumPasswordLength)
            {
                return RegisterResultCode.InvalidUsername;
            }
            
            if (notify.Password == null || notify.Password.Length is < MinimumPasswordLength or > MaximumPasswordLength)
            {
                return RegisterResultCode.InvalidPassword;
            }
            
            using var conn = new MySqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();
            var result = conn.QueryFirstOrDefault("SELECT * FROM users WHERE UserName = @Username",
                new { username = notify.Username });

            if (result != null)
            {
                return RegisterResultCode.UsernameExists;
            }

            string salt = GenerateSalt();
            string passwordHash = GetPasswordHash(notify.Password, salt);

            var rows = conn.Execute("INSERT INTO users (Username, Password, Salt) VALUES (@username, @password, @salt)", new
            {
                username = notify.Username,
                password = passwordHash,
                salt = salt
            });

            return rows == 1 ? RegisterResultCode.Success : RegisterResultCode.DatabaseError;
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