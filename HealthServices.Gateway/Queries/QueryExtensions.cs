namespace HealthServices.Gateway.Queries;

public static class QueryExtensions
{
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        return services
            .AddScoped<GetPersonByIdQueryHandler>()
            .AddScoped<GetPersonByUserNameQueryHandler>();
    }
}
