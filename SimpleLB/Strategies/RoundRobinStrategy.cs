using System.Collections.Generic;
using SimpleLB.Core;

namespace SimpleLB.Strategies
{
    public class RoundRobinStrategy : IBackendStrategy
    {
        private readonly object _lock = new object();
        private int _next = 0;

        public IBackend Select(IList<IBackend> backends)
        {
            if (backends == null || backends.Count == 0)
            {
                return null;
            }

            lock (_lock)
            {
                int index = _next % backends.Count;
                _next = (index + 1) % backends.Count;
                return backends[index];
            }
        }
    }
}
