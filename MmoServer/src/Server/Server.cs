using MmoServer.Connection;
using MmoServer.World;

namespace MmoServer
{
    public class Server
    {
        private PortListener _portListener;

        private readonly WorldService _worldService;
        private readonly ConnectionManager _connectionManager;
        
        public bool IsRunning { get; private set; }

        public Server(WorldService worldService, ConnectionManager connectionManager)
        {
            _worldService = worldService;
            _connectionManager = connectionManager;
            _portListener = new PortListener(connectionManager);
        }

        public void Start()
        {
            IsRunning = true;
            _portListener.Start();
            _worldService.Start();
        }

        public void Stop()
        {
            _portListener.Close();
            _connectionManager.Stop();
            _worldService.Stop();
            IsRunning = false;
        }
    }
}