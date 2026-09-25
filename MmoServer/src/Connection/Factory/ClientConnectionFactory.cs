using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace MmoServer.Connection.Factory;

public class ClientConnectionFactory : IClientConnectionFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ClientConnectionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ClientConnection Create(TcpClient client, uint connectionIndex)
    {
        return ActivatorUtilities.CreateInstance<ClientConnection>(
            _serviceProvider,
            client,
            connectionIndex);
    }
}