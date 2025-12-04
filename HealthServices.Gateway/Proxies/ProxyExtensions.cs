namespace HealthServices.Gateway.Proxies;

public static class ProxyExtensions
{
    public static IServiceCollection AddProxies(this IServiceCollection services)
    {
        return services
            .AddKeycloakAdminProxy()
            .AddPersonServiceProxy();
    }
}
