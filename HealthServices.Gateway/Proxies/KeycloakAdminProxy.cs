using HealthServices.Gateway.Commands;
using HealthServices.Keycloak.Boundary.Commands;
using HealthServices.Keycloak.Boundary.Values;
using HealthServices.ServiceDefaults.Helpers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HealthServices.Gateway.Proxies;

public class KeycloakAdminProxy(HttpClient httpClient, ILogger<KeycloakAdminProxy> logger, IConfiguration config)
{
    private static readonly JsonSerializerOptions _camelCaseOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly string _realm = config["Keycloak:Realm"] ?? "health";

    public async Task<Result> CreateUser(CreateUserCommand command, string? bearerToken, CancellationToken cancellationToken)
    {
        var createKeycloakUserCommand = new CreateKeycloakUserCommand()
        {
            Username = command.Username,
            FirstName = string.Join(' ', command.Given),
            LastName = command.Family,
            Email = command.Email,
            Enabled = true,
            EmailVerified = true,
            Credentials = [new KeycloakCredential
                {
                    Type = "password",
                    Value = command.Password,
                    Temporary = false
                }]
        };

        var json = JsonSerializer.Serialize(createKeycloakUserCommand, _camelCaseOptions);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/admin/realms/{_realm}/users")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        AddBearerToken(bearerToken, httpRequest);

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (response.IsSuccessStatusCode)
            return Result.Success();

        int statusCode = (int)response.StatusCode;
        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if ((int)(statusCode / 100) == 4)
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("Client error when creating Keycloak user. Status Code: {StatusCode}, Error: {ErrorContent}", response.StatusCode, errorContent);
        }
        else
            logger.LogError("Server error when creating Keycloak user. Status Code: {StatusCode}, Error: {ErrorContent}", response.StatusCode, errorContent);

        return Result.Failure(response.StatusCode, errorContent);
    }

    public async Task<Result> DeleteUser(string userName, string? bearerToken, CancellationToken cancellationToken)
    {
        using var httpGetRequest = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users/?username={userName}");
        AddBearerToken(bearerToken, httpGetRequest);

        var getResponse = await httpClient.SendAsync(httpGetRequest, cancellationToken);
        if (!getResponse.IsSuccessStatusCode)
            return Result.Failure(getResponse.StatusCode, $"Unable to retrieve user with user name {userName}");

        var userIds = await getResponse.Content.ReadFromJsonAsync<List<KeycloakUserId>>(_camelCaseOptions, cancellationToken);

        using var httpDeleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/admin/realms/{_realm}/users/{userIds?.FirstOrDefault()?.Id}");
        AddBearerToken(bearerToken, httpDeleteRequest);

        var deleteResponse = await httpClient.SendAsync(httpDeleteRequest, cancellationToken);
        if (!deleteResponse.IsSuccessStatusCode)
            return Result.Failure(deleteResponse.StatusCode, $"Unable to delete user with user name {userName}");

        return Result.Success();
    }

    private static void AddBearerToken(string? bearerToken, HttpRequestMessage httpRequest)
    {
        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            var tokenValue = bearerToken.Trim();

            tokenValue = tokenValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ?
                tokenValue["Bearer ".Length..].Trim() : tokenValue;

            httpRequest.Headers.Authorization = !string.IsNullOrEmpty(tokenValue) ?
                new AuthenticationHeaderValue("Bearer", tokenValue) : null;
        }
    }
}

public class KeycloakUserId
{
    public Guid Id { get; set; }
}

public static class KeycloakAdminProxyExtensions
{
    public static IServiceCollection AddKeycloakAdminProxy(this IServiceCollection services)
    {
        services.AddHttpClient<KeycloakAdminProxy>(client =>
        {
            client.BaseAddress = new("https+http://keycloak");
        });

        return services;
    }
}