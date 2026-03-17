namespace GnSecurity.Jwt;

/// <summary>
/// Claims utilisateur necessaires pour la generation de tokens JWT.
/// Utilise par <see cref="IUserClaimsProvider"/> lors du rafraichissement de tokens.
/// </summary>
/// <param name="UserId">Identifiant unique de l'utilisateur.</param>
/// <param name="Email">Adresse email de l'utilisateur.</param>
/// <param name="FullName">Nom complet de l'utilisateur.</param>
/// <param name="Roles">Liste des roles de l'utilisateur.</param>
/// <param name="Permissions">Liste des permissions de l'utilisateur.</param>
public record UserClaims(
    int UserId,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions
);
