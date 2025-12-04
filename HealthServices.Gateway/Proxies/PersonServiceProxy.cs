using HealthServices.Persons.Boundary.Values;
using HealthServices.ServiceDefaults.Helpers;
using System.Text;
using System.Text.Json;

namespace HealthServices.Gateway.Proxies;

public class PersonServiceProxy(HttpClient httpClient, ILogger<PersonServiceProxy> logger)
{
    public async Task<Result> CreatePersonForUser(PersonProfile person, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(person);
        var responseMessage = await httpClient.PostAsync("/person", new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);
        if (responseMessage.IsSuccessStatusCode)
            return Result.Success();

        var errorContent = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        logger.LogError("Failed to save person. Status Code: {StatusCode}, StatusMessage: {StatusMessage}", responseMessage.StatusCode, errorContent);

        return Result.Failure(responseMessage.StatusCode, errorContent);
    }

    public async Task<PersonProfile?> GetPersonById(Guid id, CancellationToken cancellationToken)
    {
        return await httpClient.GetFromJsonAsync<PersonProfile>($"/person/{id}", cancellationToken);
    }

    public async Task<PersonProfile?> GetPersonByUserName(string userName, CancellationToken cancellationToken)
    {
        var persons = await httpClient.GetFromJsonAsync<List<PersonProfile>>($"/person/?username={userName}", cancellationToken);
        if (persons?.Count > 1)
            logger.LogWarning("Multiple persons found for username {UserName}", userName);

        return persons?.FirstOrDefault();
    }
}

public static class PersonServiceProxyExtensions
{
    public static IServiceCollection AddPersonServiceProxy(this IServiceCollection services)
    {
        services.AddHttpClient<PersonServiceProxy>(config =>
        {
            config.BaseAddress = new("https+http://healthservices-personservice");
        });
        return services;
    }
}