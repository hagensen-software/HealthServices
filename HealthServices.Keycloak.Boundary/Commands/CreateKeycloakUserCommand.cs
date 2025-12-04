using HealthServices.Keycloak.Boundary.Values;

namespace HealthServices.Keycloak.Boundary.Commands;

public record CreateKeycloakUserCommand
{
    public string Username { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public bool Enabled { get; init; } = true;
    public bool EmailVerified { get; init; } = true;
    public List<KeycloakCredential>? Credentials { get; init; }
    public List<string>? RealmRoles { get; init; }
    public List<string>? ClientRoles { get; init; }
}

