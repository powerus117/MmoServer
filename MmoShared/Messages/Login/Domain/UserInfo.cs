namespace MmoShared.Messages.Login.Domain
{
    public class UserInfo
    {
        public ulong UserId { get; }
        public string Username { get; }

        public UserInfo(ulong userId, string username)
        {
            UserId = userId;
            Username = username;
        }
    }
}