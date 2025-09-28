namespace SimpleLB.Config
{
    public interface IConfigurationProvider
    {
        LoadBalancerConfigurations GetConfigurations();
    }
}
