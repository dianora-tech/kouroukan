namespace GnSecurity.Hashing;

/// <summary>
/// Service de hachage de mots de passe.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hache un mot de passe en utilisant un sel aleatoire.
    /// </summary>
    /// <param name="password">Mot de passe en clair.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Hash du mot de passe au format "{salt_base64}:{hash_base64}".</returns>
    Task<string> HashAsync(string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifie qu'un mot de passe correspond a un hash.
    /// </summary>
    /// <param name="password">Mot de passe en clair a verifier.</param>
    /// <param name="hashedPassword">Hash stocke au format "{salt_base64}:{hash_base64}".</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns><c>true</c> si le mot de passe correspond, <c>false</c> sinon.</returns>
    Task<bool> VerifyAsync(string password, string hashedPassword, CancellationToken cancellationToken = default);
}
