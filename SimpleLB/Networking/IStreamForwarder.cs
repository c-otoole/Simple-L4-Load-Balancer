using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleLB.Networking
{
    public interface IStreamForwarder
    {
        Task CopyAsync(Stream source, Stream destination, CancellationToken cancellationToken);
    }
}
