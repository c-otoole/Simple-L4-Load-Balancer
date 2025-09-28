using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleLB.Config;
using SimpleLB.Core;
using SimpleLB.Networking;
using SimpleLB.Services;

namespace SimpleLB
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var appConfigProvider = new AppConfigProvider();
            var configurations = appConfigProvider.GetConfigurations();
            var backends = new BackendPool(configurations.Backends);

            var strategyFactory = new StrategyFactory();
            var strategy = strategyFactory.CreateBackendStrategy(configurations.StrategyName);
            var loadBalancerServer = new LoadBalancerService(configurations.ListeningPort, backends, strategy, configurations.ConnectTimeoutMs, new TcpProxy(new StreamForwarder()));
            var healthChecker = new HealthChecker(backends,  configurations.HealthIntervals, configurations.ConnectTimeoutMs);

            var cancellationToken = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, arg) =>
            {
                arg.Cancel = true;
                cancellationToken.Cancel();
            };

            Console.WriteLine("Starting load balancer. Press Ctrl+C to stop");

            await Task.WhenAll(loadBalancerServer.RunAsync(cancellationToken.Token), healthChecker.RunAsync(cancellationToken.Token));
        }
    }
}

