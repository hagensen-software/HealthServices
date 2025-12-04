using HealthServices.Persons.Boundary.Commands;
using HealthServices.Persons.Boundary.Values;
using HealthServices.PersonService.Commands;
using HealthServices.PersonService.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HealthServices.PersonService;

public struct QueryParameters
{
    public string UserName { get; set; }
}

public static class EndpointExtension
{
    public static IApplicationBuilder MapPersonEndpoints(this WebApplication app)
    {
        app.MapGet("/person/",
            async ([AsParameters] QueryParameters parameters, [FromServices] GetPersonByUserNameQueryHandler queryHandler, CancellationToken cancellationToken)
            => await queryHandler.Handle(parameters.UserName, cancellationToken));

        app.MapGet("/person/{id:guid}",
            async (Guid id, [FromServices] GetPersonByIdQueryHandler queryHandler, CancellationToken cancellationToken)
            => await queryHandler.Handle(new(id), cancellationToken));

        app.MapPost("/person",
            async (PersonProfile person, SavePersonCommandHandler commandHandler, CancellationToken cancellationToken)
            => await commandHandler.Handle(new SavePersonCommand(person), cancellationToken));

        return app;
    }
}
