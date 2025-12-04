using HealthServices.Persons.Boundary.Queries;
using HealthServices.Persons.Boundary.Values;
using HealthServices.PersonService.Entities;
using Marten;

namespace HealthServices.PersonService.Queries;

public class GetPersonByIdQueryHandler(IDocumentSession session)
{
    public async Task<IResult> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var personEntity = await session.LoadAsync<PersonEntity>(request.Id, cancellationToken);
        if (personEntity == null)
            return Results.NotFound();

        return Results.Json<PersonProfile>(new PersonProfile(
            new PersonId(
                request.Id.ToString()),
            personEntity.UserName,
            personEntity.Identifiers,
            personEntity.Active,
            personEntity.Name,
            personEntity.Telecoms,
            personEntity.Addresses));
    }
}
