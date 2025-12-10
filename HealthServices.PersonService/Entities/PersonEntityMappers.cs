using HealthServices.Persons.Boundary.Values;
using System.Collections.Immutable;

namespace HealthServices.PersonService.Entities;

public static class PersonEntityMappers
{
    public static PersonEntity ToEntity(this PersonProfile profile)
    {
        return new PersonEntity(
            Guid.Parse(profile.Id.Value),
            profile.UserName?.ToEntity(),
            profile.Identifiers.Select(i => i.ToEntity()).ToImmutableList(),
            profile.Active.ToEntity(),
            profile.Name.ToEntity(),
            profile.Telecoms.Select(t => t.ToEntity()).ToImmutableList(),
            profile.Addresses.Select(a => a.ToEntity()).ToImmutableList()
        );
    }

    public static PersonProfile ToProfile(this PersonEntity entity)
    {
        return new PersonProfile(
            new PersonId(entity.Id.ToString()),
            entity.UserName?.ToProfile(),
            entity.Identifiers.Select(i => i.ToProfile()).ToImmutableList(),
            entity.Active.ToProfile(),
            entity.Name.ToProfile(),
            entity.Telecoms.Select(t => t.ToProfile()).ToImmutableList(),
            entity.Addresses.Select(a => a.ToProfile()).ToImmutableList()
        );
    }

    // Boundary -> Entity
    public static PersonIdentifierEntity ToEntity(this PersonIdentifier id)
        => new PersonIdentifierEntity(id.System, id.Value, id.Period);

    public static PersonActiveEntity ToEntity(this PersonActive a)
        => new PersonActiveEntity(a.Value);

    public static PersonNameEntity ToEntity(this PersonName n)
        => new PersonNameEntity(n.Use, n.Text, n.Family, n.Given, n.Prefix, n.Suffix, n.Period);

    public static PersonTelecomEntity ToEntity(this PersonTelecom t)
        => new PersonTelecomEntity(t.System, t.Value);

    public static PersonAddressEntity ToEntity(this PersonAddress a)
        => new PersonAddressEntity(a.Lines, a.City, a.State, a.PostalCode, a.Country, a.Period);

    public static UserNameIdentifierEntity ToEntity(this UserNameIdentifier u)
        => new UserNameIdentifierEntity(u.Value);

    // Entity -> Boundary
    public static PersonIdentifier ToProfile(this PersonIdentifierEntity id)
        => new PersonIdentifier(id.System, id.Value, id.Period);

    public static PersonActive ToProfile(this PersonActiveEntity a)
        => new PersonActive(a.Value);

    public static PersonName ToProfile(this PersonNameEntity n)
        => new PersonName(n.Use, n.Text, n.Family, n.Given, n.Prefix, n.Suffix, n.Period);

    public static PersonTelecom ToProfile(this PersonTelecomEntity t)
        => new PersonTelecom(t.System, t.Value);

    public static PersonAddress ToProfile(this PersonAddressEntity a)
        => new PersonAddress(a.Lines, a.City, a.State, a.PostalCode, a.Country, a.Period);

    public static UserNameIdentifier ToProfile(this UserNameIdentifierEntity u)
        => new UserNameIdentifier(u.Value);
}
