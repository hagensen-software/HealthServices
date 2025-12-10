# Service implementation patterns and conventions

Use these guidelines when implementing services to match the existing projects (Gateway and PersonService).

Project layout and folders
- Follow the simple, consistent folder layout: `Commands/`, `Queries/`, `Entities/`, `EndpointExtensions` (or `EndpointExtension.cs`), `CommandHandlers` / `QueryHandlers`, and `Helpers`.
- Keep DTOs and shared models in boundary projects (for example `HealthServices.Persons.Boundary`) and reference them from service projects.

Program.cs / minimal API bootstrap
- Call `builder.AddServiceDefaults()` early in `Program.cs` to apply common configuration.
- Register service-specific DI with extension methods, e.g. `builder.Services.AddOpenApi();`, `builder.Services.AddQueries();`, `builder.Services.AddCommands();`.
- In the app, call `app.MapDefaultEndpoints();` and then service-specific endpoint extension methods like `app.MapPersonEndpoints();`.
- Use `app.UseHttpsRedirection();` and map the OpenAPI UI only in development (`if (app.Environment.IsDevelopment()) app.MapOpenApi();`).

Endpoint patterns
- Implement endpoints as extension methods to the `WebApplication` (or `IApplicationBuilder`) in a static `EndpointExtension` class.
- Use minimal API signatures that accept parameters, DI, and `CancellationToken` directly. Example pattern:
  `app.MapGet("/person/{id:guid}", async (Guid id, [FromServices] IQueryHandler<GetPersonByIdQuery, Person> queryHandler, CancellationToken cancellationToken) => await queryHandler.Handle(new(id), cancellationToken));`
- Keep endpoint logic minimal: call the appropriate command/query handler and return the result.
- Endpoints in the fhir group should convert results to FHIR JSON using helper methods (see FHIR response handling below).
- Return FHIR responses as `Results.Content(fhirString, "application/json")` or appropriate `Results.StatusCode(...)` when upstream calls fail.

Dependency registration (commands/queries)
- Provide static extension methods `AddQueries(this IServiceCollection services)` and `AddCommands(this IServiceCollection services)` in each service project that register the corresponding handlers:
  `services.AddScoped<IQueryHandler<GetPersonByIdQuery, Person>, GetPersonByIdQueryHandler>();`
- Keep registrations minimal and scoped as in the existing projects.

Proxies and HTTP clients (Gateway)
- For any external service the gateway calls (person service, keycloak), create a typed proxy class and register it with `IHttpClientFactory`:
  `services.AddHttpClient<PersonServiceProxy>(config => config.BaseAddress = new Uri("https+http://healthservices-personservice"));`
- Proxy methods should use `HttpClient` to call the downstream service, check `IsSuccessStatusCode`, read content and convert to FHIR JSON using the FHIR helper methods described below.

FHIR response handling
- Use the `FhirResponseHelper` helper pattern to separate concerns:
  1. `GetResourceAsync<TResource>(string query, HttpContent content, CancellationToken cancellationToken, ILogger logger)` — deserializes `HttpContent` to the resource type and logs JSON deserialization errors; returns the resource or `null`.
  2. `HandleSerializationResult<TResource>(string query, (string? FhirJson, OperationOutcome Outcome) serializationResult, ILogger logger)` — accepts the tuple returned by the resource's serialization method and logs any serialization/operation outcome errors; returns the FHIR JSON string or `null`.
- Calling pattern in gateway proxies:
  - Call `var resource = await FhirResponseHelper.GetResourceAsync<Person>(query, response.Content, cancellationToken, logger);`
  - If `resource` is `null` then return `Results.InternalServerError()` (or appropriate error)
  - Call the resource serializer: `var serializationResult = resource.ToFhirJsonString();`
  - Call `var fhirString = FhirResponseHelper.HandleSerializationResult<Person>(query, serializationResult, logger);`
  - If `fhirString` is `null` return an error, otherwise `return Results.Content(fhirString, "application/json");`

Error handling and logging
- Log failures with `ILogger` including context (`ResourceType`, `Query`, `StatusCode` or diagnostics).
- Log internal errors at `LogLevel.Error` and upstream HTTP call failures at `LogLevel.Warning`.
- For failed upstream HTTP calls log the status code and body (response content) before returning a `Result` or `IResult` that represents the failure.
- Use the `Result` / `Result<T>` pattern from `HealthServices.ServiceDefaults.Helpers` for internal command/operation success or failure values.

Async / Cancellation
- All IO-bound methods should be async and accept a `CancellationToken` that flows to `HttpClient` calls and handlers.

Other conventions
- Prefer minimal and focused changes: add small extension methods and classes that follow the existing naming and DI patterns.
- Keep public boundary contracts in the boundary projects; implementation details remain in the service project.
- Prefer record types for commands, queries, and DTOs.
- Follow existing naming conventions (e.g., `GetPersonByIdQuery`, `GetPersonByIdQueryHandler`, `MapPersonEndpoints`, etc.).
- When adding new infrastructure (e.g., Postgres), expose configuration via environment variables or DI and make the AppHost reference the infra service (see `AppHost` `WithReference(postgres)` pattern).

Follow these patterns when adding new services or updating existing ones so code stays consistent with the Gateway and PersonService projects.
