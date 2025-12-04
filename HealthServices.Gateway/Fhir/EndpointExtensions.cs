using HealthServices.Gateway.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HealthServices.Gateway.Fhir;

public static class EndpointExtensions
{
    public static WebApplication MapFhirEndpoints(this WebApplication app)
    {
        var fhirApi = app.MapGroup("/fhir");

        fhirApi.MapGet("/Person/{id:guid}",
            async (
                Guid id,
                [FromServices] GetPersonByIdQueryHandler queryHandler,
                CancellationToken cancellationToken) =>
            await queryHandler.Handle(
                new GetPersonByIdQuery(id, false),
                cancellationToken))
            .RequireAuthorization();

        return app;
    }
}
