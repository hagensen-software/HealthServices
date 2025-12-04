using HealthServices.PersonService;
using HealthServices.PersonService.Commands;
using HealthServices.PersonService.Queries;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDataSource("postgres");

builder.AddServiceDefaults();

builder.Services
    .AddOpenApi()
    .AddCommands()
    .AddQueries();

builder.Services
    .AddMarten(options =>
    {
        options.DatabaseSchemaName = "person_db";

        //options.AutoCreateSchemaObjects = AutoCreate.All;
        //options.Schema.For<Person>();
    })
    .UseLightweightSessions()
    .UseNpgsqlDataSource();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapPersonEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
