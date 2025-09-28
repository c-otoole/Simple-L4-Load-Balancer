namespace SimpleLB.Core
{
    public class Backend : IBackend
    {
        public string Host { get; }
        public int Port { get; }

        private bool _isHealthy = true;

        private readonly object _lock = new object();

        public bool IsHealthy
        {
            get
            {
                lock (_lock)
                {
                    return _isHealthy;
                }
            }
            set
            {
                lock (_lock)
                {
                    _isHealthy = value;
                }
            }
        }

        public Backend(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
