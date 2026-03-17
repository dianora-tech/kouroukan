using System.Security.Cryptography;

namespace GnSecurity.Encryption;

/// <summary>
/// Service de chiffrement symetrique AES-256-GCM avec derivation de cle PBKDF2.
/// <para>
/// Format de sortie Base64 : [salt 16 octets][nonce 12 octets][tag 16 octets][ciphertext N octets].
/// </para>
/// </summary>
public sealed class AesEncryptionService : IAesEncryptionService
{
    /// <summary>Taille du sel PBKDF2 en octets (128 bits).</summary>
    private const int SaltSize = 16;

    /// <summary>Taille du nonce AES-GCM en octets (96 bits).</summary>
    private const int NonceSize = 12;

    /// <summary>Taille du tag d'authentification AES-GCM en octets (128 bits).</summary>
    private const int TagSize = 16;

    /// <summary>Taille de la cle derivee en octets (256 bits pour AES-256).</summary>
    private const int KeySize = 32;

    /// <summary>Nombre d'iterations PBKDF2.</summary>
    private const int Pbkdf2Iterations = 100_000;

    /// <summary>Algorithme de hachage pour PBKDF2.</summary>
    private static readonly HashAlgorithmName Pbkdf2HashAlgorithm = HashAlgorithmName.SHA256;

    /// <inheritdoc />
    public string Encrypt(string plainText, string passphrase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plainText, nameof(plainText));
        ArgumentException.ThrowIfNullOrWhiteSpace(passphrase, nameof(passphrase));

        var plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);

        var key = DeriveKey(passphrase, salt);

        var ciphertext = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Encrypt(nonce, plainBytes, ciphertext, tag);

        // Format : salt + nonce + tag + ciphertext
        var result = new byte[SaltSize + NonceSize + TagSize + ciphertext.Length];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(nonce, 0, result, SaltSize, NonceSize);
        Buffer.BlockCopy(tag, 0, result, SaltSize + NonceSize, TagSize);
        Buffer.BlockCopy(ciphertext, 0, result, SaltSize + NonceSize + TagSize, ciphertext.Length);

        return Convert.ToBase64String(result);
    }

    /// <inheritdoc />
    public string Decrypt(string cipherText, string passphrase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cipherText, nameof(cipherText));
        ArgumentException.ThrowIfNullOrWhiteSpace(passphrase, nameof(passphrase));

        byte[] data;

        try
        {
            data = Convert.FromBase64String(cipherText);
        }
        catch (FormatException ex)
        {
            throw new CryptographicException("Le texte chiffre n'est pas un Base64 valide.", ex);
        }

        var minimumLength = SaltSize + NonceSize + TagSize;
        if (data.Length < minimumLength)
        {
            throw new CryptographicException("Le texte chiffre est trop court pour etre valide.");
        }

        // Extraire salt, nonce, tag et ciphertext
        var salt = data.AsSpan(0, SaltSize).ToArray();
        var nonce = data.AsSpan(SaltSize, NonceSize).ToArray();
        var tag = data.AsSpan(SaltSize + NonceSize, TagSize).ToArray();
        var ciphertext = data.AsSpan(SaltSize + NonceSize + TagSize).ToArray();

        var key = DeriveKey(passphrase, salt);

        var plainBytes = new byte[ciphertext.Length];

        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Decrypt(nonce, ciphertext, tag, plainBytes);

        return System.Text.Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// Derive une cle AES-256 a partir d'une passphrase et d'un sel en utilisant PBKDF2-SHA256.
    /// </summary>
    /// <param name="passphrase">Phrase secrete.</param>
    /// <param name="salt">Sel cryptographique.</param>
    /// <returns>Cle de 32 octets (256 bits).</returns>
    private static byte[] DeriveKey(string passphrase, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            passphrase,
            salt,
            Pbkdf2Iterations,
            Pbkdf2HashAlgorithm);

        return pbkdf2.GetBytes(KeySize);
    }
}
