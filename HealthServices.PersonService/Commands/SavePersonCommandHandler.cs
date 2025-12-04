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
            var personEntity = new PersonEntity(
                Guid.Parse(command.Person.Id.Value),
                command.Person.UserName,
                command.Person.Identifiers,
                command.Person.Active,
                command.Person.Name,
                command.Person.Telecoms,
                command.Person.Addresses
            );

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
