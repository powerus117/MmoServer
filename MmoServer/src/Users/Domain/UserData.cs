using MmoServer.Core;

namespace MmoServer.Users.Domain
{
    public class UserData
    {
        private const string DefaultColor = "C52424";

        public Vector2I Position { get; set; }
        public string Color { get; set; } = DefaultColor;
    }
}