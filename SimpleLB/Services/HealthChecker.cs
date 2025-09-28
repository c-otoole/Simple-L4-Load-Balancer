using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleLB.Core;

namespace SimpleLB.Services
{
    public sealed class HealthChecker
    {
        private readonly IBackendPool _backendPool;
        private readonly int _intervalMs;
        private readonly int _connectTimeoutMs;

        public HealthChecker(IBackendPool backendPool, int intervalsMs, int connectTimeoutMs)
        {
            if (intervalsMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(intervalsMs));
            }

            if (connectTimeoutMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(connectTimeoutMs));
            }

            _backendPool = backendPool ?? throw new ArgumentNullException(nameof(backendPool));
            _intervalMs = intervalsMs;
            _connectTimeoutMs = connectTimeoutMs;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            // Initial log
            foreach (var backend in _backendPool.All)
            {
                var connected = await CanConnect(backend.Host, backend.Port, _connectTimeoutMs, cancellationToken).ConfigureAwait(false);
                backend.IsHealthy = connected;

                Console.WriteLine($"{DateTime.Now:HH:mm:ss} Host:{backend.Host} Port:{backend.Port} {(connected ? "UP" : "DOWN")}");
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var backend in _backendPool.All)
                {
                    var connected = await CanConnect(backend.Host, backend.Port, _connectTimeoutMs, cancellationToken).ConfigureAwait(false);

                    // Only log if changed
                    if (connected != backend.IsHealthy)
                    {
                        backend.IsHealthy = connected;
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} Host:{backend.Host} Port:{backend.Port} {(connected ? "UP" : "DOWN")}");
                    }
                }

                try
                {
                    await Task.Delay(_intervalMs, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private static async Task<bool> CanConnect(string host, int port, int timeoutMs, CancellationToken cancellationToken)
        {
            using (var client = new TcpClient())
            {
                try
                {
                    var connectTask = client.ConnectAsync(host, port);
                    var timeoutTask = Task.Delay(timeoutMs, cancellationToken);
                    var finished = await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false);

                    if (finished != connectTask)
                    {
                        return false;
                    }
                    await connectTask.ConfigureAwait(false); // may throw if refused

                    return client.Connected;
                }
                catch (SocketException)
                {
                    Console.WriteLine($"Health check failed: {host}:{port} is unreachable.");
                    return false;
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine($"Health check aborted: TcpClient for {host}:{port} was disposed");
                    return false;
                }
            }
        }
    }
}