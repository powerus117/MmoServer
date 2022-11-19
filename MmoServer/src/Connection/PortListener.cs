using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MmoServer.Connection
{
    public class PortListener
    {
        private readonly Server _server;
        private readonly Thread _listeningThread;
        private TcpListener _listener;
        private bool _listening;
        private CancellationTokenSource _cancellationTokenSource;

        public PortListener(Server server)
        {
            _server = server;
            _listeningThread = new Thread(ListenerThread);
            _listener = new(IPAddress.Any, 7800);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _listening = true;
            _listener.Start();
            _listeningThread.Start();
            
            Console.WriteLine("Started listing for connections...");
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
                    Console.WriteLine("New connection from " + client.Client.RemoteEndPoint);

                    _server.CreatePlayer(client);
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Listener cancelled");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _listener.Stop();
            }
        }
    }
}