using HealthCTX.Domain.Attributes;
using HealthCTX.Domain.Persons;

namespace HealthServices.Persons.Boundary.Values.Interfaces;

[FhirFixedValue("system", "http://healthservices/keycloak")]
public interface IUserNameIdentifier : IPersonIdentifier;

[FhirValueSlicing("identifier", "system", typeof(IUserNameIdentifier), Cardinality.Optional)]
public interface IPersonProfile : IPerson;