using HealthServices.Persons.Boundary.Values;
using HealthServices.PersonService.Entities;
using Marten;

namespace HealthServices.PersonService.Queries;

public class GetPersonByUserNameQueryHandler(IDocumentSession session)
{
    public async Task<IResult?> Handle(string userName, CancellationToken cancellationToken)
    {
        var personEntity = await session.Query<PersonEntity>()
            .Where(p => p.UserName != null && p.UserName.Value.Value == userName)
            .FirstOrDefaultAsync(cancellationToken);

        if (personEntity == null)
            return Results.Json<List<PersonProfile>>([]);

        return Results.Json<List<PersonProfile>>([personEntity.ToProfile()]);
    }
}
