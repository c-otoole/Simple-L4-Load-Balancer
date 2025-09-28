using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleLB.Networking
{
    public class StreamForwarder : IStreamForwarder
    {
        private const int BufferSize = 8192;

        public async Task CopyAsync(Stream source, Stream destination, CancellationToken cancellationToken)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var buffer = new byte[BufferSize];
            try
            {
                while (true)
                {
                    var read = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                    
                    if (read == 0)
                    {
                        break; // EOF
                    }

                    await destination.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                    await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Stream forwarding cancelled by request");
            }
            catch (IOException ex)
            {
                // Stream forwarding stopped, connection closed
            }
        }
    }
}
