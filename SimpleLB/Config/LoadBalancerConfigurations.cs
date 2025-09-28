using System.Collections.Generic;
using SimpleLB.Core;

namespace SimpleLB.Config
{
    public class LoadBalancerConfigurations
    {
        public LoadBalancerConfigurations(int listeningPort, int healthIntervals, int connectTimeoutMs, IEnumerable<Backend> backends, string strategyName)
        {
            ListeningPort = listeningPort;
            HealthIntervals = healthIntervals;
            ConnectTimeoutMs = connectTimeoutMs;
            Backends = backends;
            StrategyName = strategyName;
        }

        public int ListeningPort { get; set; }

        public int HealthIntervals { get; set; }
        
        public int ConnectTimeoutMs { get; set; }
        
        public string StrategyName { get; set; }
        
        public IEnumerable<Backend> Backends { get; set; }
    }
}
