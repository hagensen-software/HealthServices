using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithHostPort(50889);

var keycloak = builder.AddKeycloak("keycloak", 8443)
    .WithDataVolume()
    .WithExternalHttpEndpoints();

var personService = builder.AddProject<HealthServices_PersonService>("healthservices-personservice")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddProject<HealthServices_Gateway>("healthservices-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(keycloak)
    .WithReference(personService)
    .WaitFor(keycloak)
    .WaitFor(personService);

builder.Build().Run();
