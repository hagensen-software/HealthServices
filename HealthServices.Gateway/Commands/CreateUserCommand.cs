using HealthCTX.Domain.ContactPoints.Model;
using HealthCTX.Domain.HumanNames.Model;
using HealthCTX.Domain.Identifiers.Model;
using HealthServices.Gateway.Proxies;
using HealthServices.Persons.Boundary.Values;
using System.Collections.Immutable;

namespace HealthServices.Gateway.Commands;

public record CreateUserCommand(
    string Username,
    string Password,
    string[] Given,
    string Family,
    string Email);

public class CreateUserCommandHandler(
    KeycloakAdminProxy keycloakAdmin,
    PersonServiceProxy personService,
    ILogger<CreateUserCommandHandler> logger)
{
    private const string keycloakSystem = "http://healthservices/keycloak";
    private const string emailSystem = "email";

    public async Task<IResult> Handle(
        CreateUserCommand command,
        string? authHeader,
        CancellationToken cancellationToken)
    {
        var person = await personService.GetPersonByUserName(command.Username, cancellationToken);

        person = Merge(command, person);

        var result = await keycloakAdmin.CreateUser(command, authHeader, cancellationToken);
        if (result.IsFailure)
            return Results.Problem(result.StatusMessage, statusCode: (int)result.StatusCode);

        var saveResult = await personService.CreatePersonForUser(person, cancellationToken);
        if (saveResult.IsSuccess)
            return Results.Ok();

        var deleteResult = await keycloakAdmin.DeleteUser(command.Username, authHeader, cancellationToken);

        if (deleteResult.IsFailure)
        {
            logger.LogError(
                "Failed to rollback user creation for {Username}: {ErrorMessage}",
                command.Username,
                deleteResult.StatusMessage);
        }

        return Results.Problem(saveResult.StatusMessage, statusCode: (int?)saveResult.StatusCode);
    }

    private PersonProfile? Merge(CreateUserCommand command, PersonProfile? person)
    {
        PersonId id = person?.Id ?? new PersonId(Guid.NewGuid().ToString());
        ImmutableList<PersonTelecom> telecoms = [
            new PersonTelecom(
                new ContactPointSystem(emailSystem),
                new ContactPointValue(command.Email)),
            ..person?.Telecoms.Where(t => (t.System.Value != emailSystem) || (t.Value.Value != command.Email)) ?? []];

        return new PersonProfile(
            id,
            new UserNameIdentifier(new IdentifierValue(command.Username)),
            person?.Identifiers ?? [],
            new PersonActive(true),
            new PersonName(
                new HumanNameUse("official"),
                new HumanNameText(string.Join(' ', [.. command.Given, command.Family])),
                new HumanNameFamily(command.Family),
                command.Given.Select(g => new HumanNameGiven(g)).ToImmutableList(),
                null, null, null),
            telecoms,
            person?.Addresses ?? []);
    }
}