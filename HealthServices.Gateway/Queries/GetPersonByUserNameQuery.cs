using HealthServices.Gateway.Proxies;

namespace HealthServices.Gateway.Queries;

public record GetPersonByUserNameQuery(string UserName);

public class GetPersonByUserNameQueryHandler(PersonServiceProxy personService)
{
    public async Task<IResult> Handle(GetPersonByUserNameQuery query, CancellationToken cancellationToken)
    {
        var person = await personService.GetPersonByUserName(query.UserName, cancellationToken);
        if (person is null)
            return Results.Empty;

        return Results.Ok(person);
    }
}