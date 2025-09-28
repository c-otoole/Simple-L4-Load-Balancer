using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleLB.Networking
{
    public interface ITcpProxy
    {
        Task ProxyAsync(NetworkStream clientStream, NetworkStream serverStream, CancellationToken cancellationToken);
    }
}
