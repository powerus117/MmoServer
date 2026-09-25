using System.Net.Sockets;

namespace MmoServer.Connection.Factory;

public interface IClientConnectionFactory
{
    ClientConnection Create(TcpClient client, uint connectionIndex);
}