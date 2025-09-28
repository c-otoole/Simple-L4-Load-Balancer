using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleLB.Networking
{
    public class TcpProxy : ITcpProxy
    {
        private readonly IStreamForwarder _streamForwarder;

        public TcpProxy(IStreamForwarder streamForwarder)
        {
            _streamForwarder = streamForwarder ?? throw new ArgumentNullException(nameof(streamForwarder));
        }

        public async Task ProxyAsync(NetworkStream clientStream, NetworkStream serverStream, CancellationToken cancellationToken)
        {
            if (clientStream == null)
            {
                throw new ArgumentNullException(nameof(clientStream));
            }

            if (serverStream == null)
            {
                throw new ArgumentNullException(nameof(serverStream));
            }

            var up = Task.Run(() => _streamForwarder.CopyAsync(clientStream, serverStream, cancellationToken));
            var down = Task.Run(() => _streamForwarder.CopyAsync(serverStream, clientStream, cancellationToken));

            await Task.WhenAll(up, down).ConfigureAwait(false);
        }
    }
}
