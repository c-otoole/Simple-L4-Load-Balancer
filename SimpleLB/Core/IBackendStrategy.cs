using System.Collections.Generic;

namespace SimpleLB.Core
{
    public interface IBackendStrategy
    {
        IBackend Select(IList<IBackend> backends);
    }
}
