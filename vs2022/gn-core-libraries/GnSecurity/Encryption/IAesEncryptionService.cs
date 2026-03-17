namespace GnSecurity.Encryption;

/// <summary>
/// Service de chiffrement symetrique AES-256-GCM.
/// </summary>
public interface IAesEncryptionService
{
    /// <summary>
    /// Chiffre un texte en clair avec AES-256-GCM.
    /// </summary>
    /// <param name="plainText">Texte en clair a chiffrer.</param>
    /// <param name="passphrase">Phrase secrete pour la derivation de cle (PBKDF2).</param>
    /// <returns>Texte chiffre encode en Base64 (format: salt + nonce + tag + ciphertext).</returns>
    string Encrypt(string plainText, string passphrase);

    /// <summary>
    /// Dechiffre un texte chiffre avec AES-256-GCM.
    /// </summary>
    /// <param name="cipherText">Texte chiffre encode en Base64.</param>
    /// <param name="passphrase">Phrase secrete utilisee lors du chiffrement.</param>
    /// <returns>Texte en clair dechiffre.</returns>
    /// <exception cref="System.Security.Cryptography.CryptographicException">
    /// Si le texte chiffre est altere ou si la passphrase est incorrecte.
    /// </exception>
    string Decrypt(string cipherText, string passphrase);
}
