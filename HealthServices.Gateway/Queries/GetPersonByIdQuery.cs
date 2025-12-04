using HealthServices.Gateway.Helpers;
using HealthServices.Gateway.Proxies;
using HealthServices.Persons.Boundary.Values;

namespace HealthServices.Gateway.Queries;

public record GetPersonByIdQuery(Guid Id, bool AsUser);

public class GetPersonByIdQueryHandler(PersonServiceProxy personService, ILogger<GetPersonByIdQueryHandler> logger)
{
    public async Task<IResult> Handle(GetPersonByIdQuery query, CancellationToken cancellationToken)
    {
        var person = await personService.GetPersonById(query.Id, cancellationToken);
        if (person is null)
            return Results.Empty;

        if (query.AsUser)
        {
            return Results.Ok(person);
        }
        else
        {
            person = person with { UserName = null };

            return Results.Content(
                FhirResponseHelper.HandleSerializationResult<PersonProfile>(
                    person.ToFhirJsonString(),
                    logger),
                "application/json");
        }
    }
}
