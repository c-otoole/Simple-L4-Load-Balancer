using System;
using System.Configuration;
using System.Linq;
using SimpleLB.Core;

namespace SimpleLB.Config
{
    public class AppConfigProvider : IConfigurationProvider
    {
        public LoadBalancerConfigurations GetConfigurations()
        {
            var listeningPortConfig = ConfigurationManager.AppSettings["ListenPort"];
            if (!int.TryParse(listeningPortConfig, out var listeningPort))
            {
                throw new ConfigurationErrorsException($"{listeningPortConfig} must be an integer");
            }

            var healthIntervalsConfig = ConfigurationManager.AppSettings["healthIntervalsMs"];
            if (!int.TryParse(healthIntervalsConfig, out var healthIntervals))
            {
                throw new ConfigurationErrorsException($"{healthIntervalsConfig} must be an integer");
            }

            var connectTimeoutMsConfig = ConfigurationManager.AppSettings["connectTimeoutMs"];
            if (!int.TryParse(connectTimeoutMsConfig, out var connectTimeoutMs))
            {
                throw new ConfigurationErrorsException($"{connectTimeoutMsConfig} must be an integer");
            }
            
            var strategyName = (ConfigurationManager.AppSettings["Strategy"] ?? "roundrobin").Trim();


            var backendsCsv = ConfigurationManager.AppSettings["Backends"];
            if (string.IsNullOrWhiteSpace(backendsCsv))
            {
                throw new ConfigurationErrorsException($"{backendsCsv} is required.");
            }

            var backends = backendsCsv
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(s =>
                {
                    var p = s.Split(':');
                    return new Backend(p[0], int.Parse(p[1]));
                });

            return new LoadBalancerConfigurations(listeningPort, healthIntervals, connectTimeoutMs, backends, strategyName);
        }
    }
}
