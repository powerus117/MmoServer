namespace MmoShared.Messages.Login.Domain
{
    public class UserInfo
    {
        public ulong UserId { get; }
        public string Username { get; }
        public AccountType AccountType { get; }

        public UserInfo(ulong userId, string username, AccountType accountType)
        {
            UserId = userId;
            Username = username;
            AccountType = accountType;
        }
    }
}