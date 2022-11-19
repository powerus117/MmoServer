using MmoServer.Users;
using MmoShared.Messages;

namespace MmoServer.Messages
{
    public interface IMessageSubscription
    {
        public void Invoke(User user, Message message);
    }
}