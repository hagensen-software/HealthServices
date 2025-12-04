using HealthServices.Persons.Boundary.Values;
using HealthServices.PersonService.Entities;
using Marten;

namespace HealthServices.PersonService.Queries;

public class GetPersonByUserNameQueryHandler(IDocumentSession session)
{
    public async Task<IResult?> Handle(string userName, CancellationToken cancellationToken)
    {
        var personEntity = session.Query<PersonEntity>().Where(p => p.UserName.Value.Value == userName).FirstOrDefault();
        if (personEntity == null)
            return Results.Json<List<PersonProfile>>([]);

        return Results.Json<List<PersonProfile>>([new PersonProfile(
            new PersonId(
                personEntity.Id.ToString()),
            personEntity.UserName,
            personEntity.Identifiers,
            personEntity.Active,
            personEntity.Name,
            personEntity.Telecoms,
            personEntity.Addresses)]);
    }
}
