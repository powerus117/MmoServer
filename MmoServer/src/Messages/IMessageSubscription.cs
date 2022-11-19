using MmoShared.Messages;

namespace MmoServer.Messages
{
    public interface IMessageSubscription
    {
        public void Invoke(Player player, Message message);
    }
}