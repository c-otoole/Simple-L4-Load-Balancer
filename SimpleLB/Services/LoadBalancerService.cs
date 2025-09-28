using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleLB.Core;
using SimpleLB.Networking;

namespace SimpleLB.Services
{
    public sealed class LoadBalancerService
    {
        private readonly int _listenPort;
        private readonly IBackendPool _backendPool;
        private readonly IBackendStrategy _strategy;
        private readonly ITcpProxy _tcpProxy;
        private readonly int _streamTimeoutMs;

        public LoadBalancerService(int listenPort, IBackendPool pool, IBackendStrategy strategy, int streamTimeoutMs, ITcpProxy tcpProxy)
        {
            if (listenPort <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(listenPort));
            }

            if (streamTimeoutMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(streamTimeoutMs));
            }

            _listenPort = listenPort;
            _backendPool = pool ?? throw new ArgumentNullException(nameof(pool));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _streamTimeoutMs = streamTimeoutMs;
            _tcpProxy = tcpProxy ?? throw new ArgumentNullException(nameof(tcpProxy));
        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var listener = new TcpListener(IPAddress.Loopback, _listenPort);
            listener.Start();
            Console.WriteLine($"Listening on {_listenPort}");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var acceptTask = listener.AcceptTcpClientAsync();
                    var cancelTask = Task.Delay(Timeout.Infinite, cancellationToken);
                    var finished = await Task.WhenAny(acceptTask, cancelTask).ConfigureAwait(false);

                    if (finished == cancelTask)
                    {
                        throw new OperationCanceledException(cancellationToken);
                    }

                    var client = await acceptTask.ConfigureAwait(false);
                    _ = HandleClientAsync(client, cancellationToken); // fire-and-forget
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Listener cancelled by request.");

            }
            catch (ObjectDisposedException)
            {
               Console.WriteLine("Listener stopped");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Listener socket error: {ex.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }

        private async Task HandleClientAsync(TcpClient inbound, CancellationToken cancellationToken)
        {
            using (inbound)
            {
                var healthy = _backendPool.GetHealthy();
                if (healthy.Count == 0)
                {
                    return;
                }

                var target = _strategy.Select(healthy);
                if (target == null)
                {
                    return;
                }

                var outbound = new TcpClient();
                
                try
                {
                    // connect to backend
                    var connectTask = outbound.ConnectAsync(target.Host, target.Port);
                    var cancelTask = Task.Delay(Timeout.Infinite, cancellationToken);
                    
                    if (await Task.WhenAny(connectTask, cancelTask).ConfigureAwait(false) == cancelTask)
                    {
                        throw new OperationCanceledException(cancellationToken);
                    }

                    await connectTask.ConfigureAwait(false);
                }
                catch (Exception)
                {
                    _backendPool.MarkDown(target);
                    try
                    {
                        outbound.Close();
                    }
                    catch
                    {
                    }

                    return;
                }

                using (outbound)
                {
                    using (var clientStream = inbound.GetStream())
                    {
                        using (var backendStream = outbound.GetStream())
                        {
                            {
                                clientStream.ReadTimeout = _streamTimeoutMs;
                                clientStream.WriteTimeout = _streamTimeoutMs;

                                backendStream.ReadTimeout = _streamTimeoutMs;
                                backendStream.WriteTimeout = _streamTimeoutMs;

                                try
                                {
                                    Console.WriteLine($"Proxying client: Port:{target.Port}, Host:{target.Host}");
                                    await _tcpProxy.ProxyAsync(clientStream, backendStream, cancellationToken);
                                }
                                catch (Exception ex)
                                {
                                    // IO failure mid transfer
                                    _backendPool.MarkDown(target);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}