using HealthServices.Persons.Boundary.Values;
using System.Collections.Immutable;

namespace HealthServices.PersonService.Entities;

public record PersonEntity(
    Guid Id,
    UserNameIdentifier UserName,
    ImmutableList<PersonIdentifier> Identifiers,
    PersonActive Active,
    PersonName Name,
    ImmutableList<PersonTelecom> Telecoms,
    ImmutableList<PersonAddress> Addresses);
