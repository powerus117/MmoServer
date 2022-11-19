namespace MmoShared.Messages.Login.Register
{
    public enum RegisterResultCode
    {
        Success,
        InternalServerError,
        UsernameExists,
        InvalidUsername,
        InvalidPassword,
        DatabaseError
    }
}