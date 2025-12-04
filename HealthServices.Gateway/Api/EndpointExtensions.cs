using HealthServices.Gateway.Commands;
using HealthServices.Gateway.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HealthServices.Gateway.Api;

public class QueryParameters
{
    public string UserName { get; set; } = null!;
}

public static class EndpointExtensions
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");

        api.MapPost("/createuser",
            async (
                HttpContext httpContext,
                [FromBody] CreateUserCommand command,
                [FromServices] CreateUserCommandHandler handler,
                CancellationToken cancellationToken) =>
            await handler.Handle(
                command,
                httpContext.Request.Headers.Authorization.FirstOrDefault(),
                cancellationToken))
            .RequireAuthorization();

        api.MapGet("/User/{id:guid}",
            async (
                Guid id,
                [FromServices] GetPersonByIdQueryHandler queryHandler,
                CancellationToken cancellationToken) =>
            await queryHandler.Handle(
                new GetPersonByIdQuery(id, true),
                cancellationToken))
            .RequireAuthorization();

        api.MapGet("/User/",
            async (
                [AsParameters] QueryParameters parameters,
                [FromServices] GetPersonByUserNameQueryHandler queryHandler,
                CancellationToken cancellationToken) =>
            await queryHandler.Handle(
                new GetPersonByUserNameQuery(parameters.UserName),
                cancellationToken))
            .RequireAuthorization();

        return app;
    }
}