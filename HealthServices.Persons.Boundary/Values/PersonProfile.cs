using HealthCTX.Domain;
using HealthCTX.Domain.Addresses.Model;
using HealthCTX.Domain.ContactPoints.Model;
using HealthCTX.Domain.HumanNames.Model;
using HealthCTX.Domain.Identifiers.Model;
using HealthCTX.Domain.Persons;
using HealthServices.Persons.Boundary.Values.Interfaces;
using System.Collections.Immutable;

namespace HealthServices.Persons.Boundary.Values;

public record PersonId(string Value) : IId;

public record PersonIdentifier(
    IdentifierSystem System,
    IdentifierValue Value,
    IdentifierPeriod? Period) : IPersonIdentifier;

public record PersonActive(bool Value) : IPersonActive;

public record PersonName(
    HumanNameUse? Use,
    HumanNameText? Text,
    HumanNameFamily? Family,
    ImmutableList<HumanNameGiven> Given,
    HumanNamePrefix? Prefix,
    HumanNameSuffix? Suffix,
    HumanNamePeriod? Period) : IPersonHumanName;

public record PersonTelecom(
    ContactPointSystem System,
    ContactPointValue Value) : IPersonContactPoint;

public record PersonAddress(
    ImmutableList<AddressLine> Lines,
    AddressCity City,
    AddressState? State,
    AddressPostalCode PostalCode,
    AddressCountry Country,
    AddressPeriod? Period) : IPersonAddress;

public record UserNameIdentifier(IdentifierValue Value) : IUserNameIdentifier;

public record PersonProfile(
    PersonId Id,
    UserNameIdentifier? UserName,
    ImmutableList<PersonIdentifier> Identifiers,
    PersonActive Active,
    PersonName Name,
    ImmutableList<PersonTelecom> Telecoms,
    ImmutableList<PersonAddress> Addresses) : IPersonProfile;
