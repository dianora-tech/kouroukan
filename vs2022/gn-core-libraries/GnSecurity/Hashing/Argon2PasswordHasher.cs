using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace GnSecurity.Hashing;

/// <summary>
/// Implementation du hachage de mots de passe avec Argon2id.
/// Parametres : DegreeOfParallelism=4, MemorySize=65536 (64 Mo), Iterations=3.
/// Format de sortie : "{salt_base64}:{hash_base64}" avec un sel de 16 octets et un hash de 32 octets.
/// </summary>
public sealed class Argon2PasswordHasher : IPasswordHasher
{
    /// <summary>Taille du sel en octets (128 bits).</summary>
    private const int SaltSize = 16;

    /// <summary>Taille du hash en octets (256 bits).</summary>
    private const int HashSize = 32;

    /// <summary>Degre de parallelisme Argon2id.</summary>
    private const int DegreeOfParallelism = 4;

    /// <summary>Memoire utilisee en Ko (64 Mo).</summary>
    private const int MemorySize = 65536;

    /// <summary>Nombre d'iterations Argon2id.</summary>
    private const int Iterations = 3;

    /// <inheritdoc />
    public async Task<string> HashAsync(string password, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = await ComputeHashAsync(password, salt, cancellationToken).ConfigureAwait(false);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    /// <inheritdoc />
    public async Task<bool> VerifyAsync(string password, string hashedPassword, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword, nameof(hashedPassword));

        var parts = hashedPassword.Split(':');
        if (parts.Length != 2)
            return false;

        byte[] salt;
        byte[] expectedHash;

        try
        {
            salt = Convert.FromBase64String(parts[0]);
            expectedHash = Convert.FromBase64String(parts[1]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualHash = await ComputeHashAsync(password, salt, cancellationToken).ConfigureAwait(false);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    /// <summary>
    /// Calcule le hash Argon2id d'un mot de passe avec un sel donne.
    /// </summary>
    /// <param name="password">Mot de passe en clair.</param>
    /// <param name="salt">Sel cryptographique.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Hash Argon2id de 32 octets.</returns>
    private static async Task<byte[]> ComputeHashAsync(string password, byte[] salt, CancellationToken cancellationToken)
    {
        var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

        using var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            MemorySize = MemorySize,
            Iterations = Iterations
        };

        return await argon2.GetBytesAsync(HashSize).WaitAsync(cancellationToken).ConfigureAwait(false);
    }
}
