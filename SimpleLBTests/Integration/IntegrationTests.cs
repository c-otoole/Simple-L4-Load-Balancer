using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleLB.Core;
using SimpleLB.Networking;
using SimpleLB.Services;
using SimpleLB.Strategies;

namespace SimpleLBTests.Integration
{
    [TestFixture]
    public class LoadBalancerIntegrationTests
    {
        [Test]
        public async Task OneBackend_EndToEnd_ProxiesTraffic()
        {
            // Arrange
            var echoBackend = new EchoBackend("B1");
            echoBackend.Start();
            var healthyBackend = new Backend("127.0.0.1", echoBackend.Port) { IsHealthy = true };
            var backendPool = new BackendPool(new[] { healthyBackend });
            var strategy = new RoundRobinStrategy();
            var streamForwarder = new StreamForwarder();
            var tcpProxy = new TcpProxy(streamForwarder);
            var freePort = GetFreePort();
            var loadBalancerService = new LoadBalancerService(freePort, backendPool, strategy, 500, tcpProxy);
            var cancellationToken = new CancellationTokenSource();
            var loadBalancerTask = loadBalancerService.RunAsync(cancellationToken.Token);

            try
            {
                // Act
                var result = await CallThroughLbAsync(freePort, "test", 2000).ConfigureAwait(false);
                // Assert
                Assert.That("B1", Is.EqualTo(result));
                Assert.That(echoBackend.Hits, Is.GreaterThanOrEqualTo(1));
            }
            finally
            {
                cancellationToken.Cancel();
                try
                {
                    await loadBalancerTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }

                echoBackend.Dispose();
                cancellationToken.Dispose();
            }
        }

        [Test]
        public async Task TwoBackends_RoundRobin_AlternatesTargets()
        {
            // Arrange
            var echoBackend1 = new EchoBackend("B1");
            var echoBackend2 = new EchoBackend("B2");
            echoBackend1.Start();
            echoBackend2.Start();

            var healthyBackend1 = new Backend("127.0.0.1", echoBackend1.Port) { IsHealthy = true };
            var healthyBackend2 = new Backend("127.0.0.1", echoBackend2.Port) { IsHealthy = true };
            var pool = new BackendPool(new[] { healthyBackend1, healthyBackend2 });
            var strategy = new RoundRobinStrategy();
            var forwarder = new StreamForwarder();
            var tcpProxy = new TcpProxy(forwarder);
            var freePort = GetFreePort();
            var loadBalancerService = new LoadBalancerService(freePort, pool, strategy, 500, tcpProxy);
            var cancellationToken = new CancellationTokenSource();
            var loadBalancerTask = loadBalancerService.RunAsync(cancellationToken.Token);
            try
            {
                // Act
                var result1 = await CallThroughLbAsync(freePort, "test1", 2000).ConfigureAwait(false);
                var result2 = await CallThroughLbAsync(freePort, "test2", 2000).ConfigureAwait(false);
                // Assert
                Assert.That("B1",Is.EqualTo(result1));
                Assert.That("B2", Is.EqualTo(result2));
                Assert.That(echoBackend1.Hits, Is.GreaterThanOrEqualTo(1));
                Assert.That(echoBackend2.Hits,Is.GreaterThanOrEqualTo(1));
            }
            finally
            {
                cancellationToken.Cancel();
                try
                {
                    await loadBalancerTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }

                echoBackend1.Dispose();
                echoBackend2.Dispose();
                cancellationToken.Dispose();
            }
        }

        private static int GetFreePort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }

        private static async Task<string> CallThroughLbAsync(int lbPort, string payload, int timeoutMs)
        {
            using (var client = new TcpClient())
            using (var cancellationToken = new CancellationTokenSource(timeoutMs))
            {
                await client.ConnectAsync(IPAddress.Loopback, lbPort).ConfigureAwait(false);
                using (var networkStream = client.GetStream())
                {
                    var bytes = Encoding.ASCII.GetBytes(payload ?? string.Empty);
                    await networkStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken.Token).ConfigureAwait(false);
                    await networkStream.FlushAsync(cancellationToken.Token).ConfigureAwait(false);
                    var buffer = new byte[64];
                    var read = await networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken.Token).ConfigureAwait(false);
                    return Encoding.ASCII.GetString(buffer, 0, read);
                }
            }
        }
    }
}


