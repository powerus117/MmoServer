using System.Net;
using System.Net.Sockets;
using MmoServer.Logging;

namespace MmoServer.Connection
{
    public class PortListener
    {
        private readonly ConnectionManager _connectionManager;
        private readonly Thread _listeningThread;
        private TcpListener _listener;
        private bool _listening;
        private CancellationTokenSource _cancellationTokenSource;

        public PortListener(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _listeningThread = new Thread(ListenerThread);
            _listener = new(IPAddress.Any, 7800);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _listening = true;
            _listener.Start();
            _listeningThread.Start();
            
            MmoLogger.Log("Started listing for connections...");
        }
        
        public void Close()
        {
            _listening = false;
            _cancellationTokenSource.Cancel();
        }

        private void ListenerThread()
        {
            try
            {
                while (_listening)
                {
                    TcpClient client = _listener.AcceptTcpClientAsync(_cancellationTokenSource.Token).Result;
                    MmoLogger.Log("New connection from " + client.Client.RemoteEndPoint);

                    _connectionManager.Add(client);
                }
            }
            catch (OperationCanceledException e)
            {
                MmoLogger.Log("Listener cancelled");
            }
            catch (Exception e)
            {
                MmoLogger.Error(e);
            }
            finally
            {
                _listener.Stop();
            }
        }
    }
}