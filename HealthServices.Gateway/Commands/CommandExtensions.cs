namespace HealthServices.Gateway.Commands;

public static class CommandExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        return services
            .AddScoped<CreateUserCommandHandler>();
    }
}
