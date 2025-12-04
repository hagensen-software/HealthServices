namespace HealthServices.Keycloak.Boundary.Values;

public record KeycloakCredential
{
    public string Type { get; init; } = "password";
    public string Value { get; init; } = null!;
    public bool Temporary { get; init; } = false;
}