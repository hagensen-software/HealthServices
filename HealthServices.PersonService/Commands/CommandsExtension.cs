namespace HealthServices.PersonService.Commands;

public static class CommandsExtension
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        return services
            .AddScoped<SavePersonCommandHandler>();
    }
}
