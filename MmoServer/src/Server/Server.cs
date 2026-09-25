using MmoServer.Connection;

namespace MmoServer
{
    public class Server
    {
        private PortListener _portListener;

        private readonly ConnectionManager _connectionManager;
        
        public bool IsRunning { get; private set; }

        public Server(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _portListener = new PortListener(connectionManager);
        }

        public void Start()
        {
            IsRunning = true;
            _portListener.Start();
        }

        public void Stop()
        {
            _portListener.Close();
            _connectionManager.Stop();
            IsRunning = false;
        }
    }
}