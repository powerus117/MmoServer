namespace MmoShared.Messages.Login.Register
{
    public enum RegisterResultCode
    {
        Success,
        UsernameExists,
        InvalidUsername,
        InvalidPassword,
        DatabaseError
    }
}