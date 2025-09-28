using System.Collections.Generic;
using System.Linq;

namespace SimpleLB.Core
{
    public class BackendPool : IBackendPool
    {
        private readonly List<IBackend> _backends;
        private readonly object _lock = new object();

        public BackendPool(IEnumerable<IBackend> backends)
        {
            _backends = backends.ToList();
        }

        public IList<IBackend> GetHealthy()
        {
            lock (_lock)
            {
                return _backends.Where(b => b.IsHealthy).ToList();
            }
        }

        public void MarkDown(IBackend backend)
        {
            backend.IsHealthy = false;
        }

        public IList<IBackend> All
        {
            get{
                lock (_lock)
                {
                    return _backends.ToList();
                }
            }
        }
    }
}
