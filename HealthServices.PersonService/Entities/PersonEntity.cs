using HealthCTX.Domain.Addresses.Model;
using HealthCTX.Domain.ContactPoints.Model;
using HealthCTX.Domain.HumanNames.Model;
using HealthCTX.Domain.Identifiers.Model;
using System.Collections.Immutable;

namespace HealthServices.PersonService.Entities;

public record PersonEntity(
    Guid Id,
    UserNameIdentifierEntity? UserName,
    ImmutableList<PersonIdentifierEntity> Identifiers,
    PersonActiveEntity Active,
    PersonNameEntity Name,
    ImmutableList<PersonTelecomEntity> Telecoms,
    ImmutableList<PersonAddressEntity> Addresses);

public record PersonIdentifierEntity(
    IdentifierSystem System,
    IdentifierValue Value,
    IdentifierPeriod? Period);

public record PersonActiveEntity(bool Value);

public record PersonNameEntity(
    HumanNameUse? Use,
    HumanNameText? Text,
    HumanNameFamily? Family,
    ImmutableList<HumanNameGiven> Given,
    HumanNamePrefix? Prefix,
    HumanNameSuffix? Suffix,
    HumanNamePeriod? Period);

public record PersonTelecomEntity(
    ContactPointSystem System,
    ContactPointValue Value);

public record PersonAddressEntity(
    ImmutableList<AddressLine> Lines,
    AddressCity City,
    AddressState? State,
    AddressPostalCode PostalCode,
    AddressCountry Country,
    AddressPeriod? Period);

public record UserNameIdentifierEntity(IdentifierValue Value);
