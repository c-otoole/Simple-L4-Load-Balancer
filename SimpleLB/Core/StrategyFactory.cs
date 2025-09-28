using System;
using System.Collections.Generic;
using SimpleLB.Strategies;

namespace SimpleLB.Core
{
    public class StrategyFactory
    {
        private readonly Dictionary<string, IBackendStrategy> _strategies = new Dictionary<string, IBackendStrategy>(StringComparer.OrdinalIgnoreCase);

        public StrategyFactory()
        {
            _strategies["RoundRobin"] = new RoundRobinStrategy();
        }

        public IBackendStrategy CreateBackendStrategy(string name)
        {
            if (!_strategies.ContainsKey(name))
            {
                throw new ArgumentException($"Unknown strategy {name}.");
            }

            return _strategies[name];
        }
}
}
