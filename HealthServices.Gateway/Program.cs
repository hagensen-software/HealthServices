using Microsoft.AspNetCore.Authentication.JwtBearer;
using HealthServices.Gateway.Api;
using HealthServices.Gateway.Fhir;
using HealthServices.Gateway.Proxies;
using HealthServices.Gateway.Commands;
using HealthServices.Gateway.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddKeycloakJwtBearer("keycloak", realm: "health", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = "account";
    });

builder.Services
    .AddAuthorization()
    .AddOpenApi()
    .AddProxies()
    .AddQueryHandlers()
    .AddCommandHandlers();

var app = builder.Build();

app .MapDefaultEndpoints()
    .MapFhirEndpoints()
    .MapApiEndpoints()
    .UseAuthentication()
    .UseAuthorization();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.Run();
