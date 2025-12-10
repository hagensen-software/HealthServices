using HealthServices.Persons.Boundary.Commands;
using HealthServices.PersonService.Entities;
using Marten;

namespace HealthServices.PersonService.Commands;

public class SavePersonCommandHandler(IDocumentSession session)
{
    public async Task<IResult> Handle(SavePersonCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var personEntity = command.Person.ToEntity();

            session.Store(personEntity);

            await session.SaveChangesAsync();

            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }
}
