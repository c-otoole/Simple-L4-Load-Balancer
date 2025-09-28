using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleLBTests.Integration
{
    public class EchoBackend : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly string _reply;
        private readonly CancellationTokenSource _cancellationToken;
        private Task _runTask;
        private int _hits;

        public int Port { get; private set; }

        public int Hits
        {
            get
            {
                return _hits;
            }
        }

        public EchoBackend(string replyTag)
            : this(replyTag, GetFreePort())
        {
        }

        public EchoBackend(string replyTag, int port)
        {
            _reply = replyTag ?? "OK";
            Port = port;
            _listener = new TcpListener(IPAddress.Loopback, Port);
            _cancellationToken = new CancellationTokenSource();
        }

        public void Start()
        {
            _listener.Start();
            _runTask = Task.Run(async () =>
            {
                var token = _cancellationToken.Token;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var acceptTask = _listener.AcceptTcpClientAsync();
                        var cancelTask = Task.Delay(Timeout.Infinite, token);
                        var finished = await Task.WhenAny(acceptTask, cancelTask).ConfigureAwait(false);
                        if (finished == cancelTask) break;
                        var accepted = await acceptTask.ConfigureAwait(false);
                        
                        // Handle each client
                        Task.Run(async () =>
                        {
                            using (accepted)
                            using (var networkStream = accepted.GetStream())
                            {
                                var buf = new byte[256];
                                try
                                {
                                    await networkStream.ReadAsync(buf, 0, buf.Length, token).ConfigureAwait(false);
                                }
                                catch
                                {
                                    // ignore read errors
                                }

                                var data = Encoding.ASCII.GetBytes(_reply);
                                await networkStream.WriteAsync(data, 0, data.Length, token).ConfigureAwait(false);
                                await networkStream.FlushAsync(token).ConfigureAwait(false);
                                Interlocked.Increment(ref _hits);
                            }
                        }, token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
            }, _cancellationToken.Token);
        }

        public void Dispose()
        {
            try
            {
                _cancellationToken.Cancel();
            }
            catch
            {
            }

            try
            {
                _listener.Stop();
            }
            catch
            {
            }

            try
            {
                if (_runTask != null) _runTask.Wait(TimeSpan.FromSeconds(1));
            }
            catch
            {
            }

            _cancellationToken.Dispose();
        }

        private static int GetFreePort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }
    }
}