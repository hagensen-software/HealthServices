namespace HealthServices.PersonService.Queries;

public static class QueriesExtension
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        return services
            .AddScoped<GetPersonByIdQueryHandler>()
            .AddScoped<GetPersonByUserNameQueryHandler>();
    }
}
