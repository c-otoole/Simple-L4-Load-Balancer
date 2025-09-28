using System.Collections.Generic;

namespace SimpleLB.Core
{
    public interface IBackendPool
    {
        IList<IBackend> GetHealthy();

        void MarkDown(IBackend backend);
        
        IList<IBackend> All { get; }

    }
}
