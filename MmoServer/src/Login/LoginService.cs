using System;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using MmoServer.Database;
using MmoServer.Messages;
using MmoServer.Users;
using MmoShared.Messages.Login;
using MmoShared.Messages.Login.Domain;
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

        private void OnLoginNotify(User user, LoginNotify notify)
        {
            var resultCode = Login(user, notify);

            user.AddMessage(new LoginResultSync
            {
                ResultCode = resultCode,
                UserInfo = user.UserInfo
            });
        }
        
        private void OnRegisterNotify(User user, RegisterNotify notify)
        {
            var resultCode = Register(user, notify);
            
            user.AddMessage(new RegisterResultSync
            {
                ResultCode = resultCode,
                UserInfo = user.UserInfo
            });
        }
        
        private LoginResultCode Login(User user, LoginNotify notify)
        {
            if (notify.Username == null || notify.Username.Length is < MinimumUsernameLength or > MaximumUsernameLength)
            {
                return LoginResultCode.InvalidCredentials;
            }
            
            if (notify.Password == null || notify.Password.Length is < MinimumPasswordLength or > MaximumPasswordLength)
            {
                return LoginResultCode.InvalidCredentials;
            }
            
            using var conn = new MySqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();
            dynamic? result;
            try
            {
                result = conn.QueryFirstOrDefault("SELECT * FROM users WHERE UserName = @Username",
                    new { username = notify.Username });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return LoginResultCode.InternalServerError;
            }

            if (result != null)
            {
                string passwordHash = GetPasswordHash(notify.Password, result.Salt);
                
                if (result.Password.Equals(passwordHash))
                {
                    user.LoadData(result.ID, result.UserName, (AccountType)result.AccountType);
                    
                    return LoginResultCode.Success;
                }
            }

            return LoginResultCode.InvalidCredentials;
        }
        
        private RegisterResultCode Register(User user, RegisterNotify notify)
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
            ulong userId = 0;

            try
            {
                userId = conn.QuerySingle<ulong>(
                    @"INSERT INTO users (Username, Password, Salt) VALUES (@username, @password, @salt); SELECT LAST_INSERT_ID();",
                    new
                    {
                        username = notify.Username,
                        password = passwordHash,
                        salt = salt
                    });
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                return RegisterResultCode.InternalServerError;
            }

            bool isSuccess = userId > 0;

            if (isSuccess)
            {
                user.LoadData(userId, notify.Username, AccountType.Normal);
            }
            
            return isSuccess ? RegisterResultCode.Success : RegisterResultCode.DatabaseError;
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