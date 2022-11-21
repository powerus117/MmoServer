using MmoServer.Messages;
using MmoServer.Users;
using MmoShared.Messages.Players;
using MmoShared.Messages.Players.Movement;

namespace MmoServer.World
{
    public class WorldService
    {
        public WorldService()
        {
            MessageManager.Instance.Subscribe<PlayerMoveNotify>(OnPlayerMoveNotify);
        }

        private void OnPlayerMoveNotify(User user, PlayerMoveNotify notify)
        {
            user.MoveToPosition(notify.Position);
        }
    }
}