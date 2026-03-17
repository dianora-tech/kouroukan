namespace GnSecurity.Jwt;

/// <summary>
/// Fournit les claims d'un utilisateur pour la generation de tokens lors du rafraichissement.
/// A implementer par le consommateur (ex: depuis la base de donnees via auth.get_user_with_roles_and_permissions).
/// </summary>
public interface IUserClaimsProvider
{
    /// <summary>
    /// Recupere les claims d'un utilisateur par son identifiant.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Les claims de l'utilisateur, ou <c>null</c> si l'utilisateur est introuvable ou inactif.</returns>
    Task<UserClaims?> GetUserClaimsAsync(int userId, CancellationToken cancellationToken = default);
}
