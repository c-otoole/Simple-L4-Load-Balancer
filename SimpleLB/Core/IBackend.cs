namespace SimpleLB.Core
{
    public interface IBackend
    {
        string Host { get; }

        int Port { get; }

        bool IsHealthy { get; set; }
    }
}
