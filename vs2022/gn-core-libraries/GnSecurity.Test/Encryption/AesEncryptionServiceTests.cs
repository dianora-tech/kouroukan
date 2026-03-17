using System.Security.Cryptography;
using FluentAssertions;
using GnSecurity.Encryption;

namespace GnSecurity.Test.Encryption;

/// <summary>
/// Tests unitaires pour <see cref="AesEncryptionService"/>.
/// </summary>
public sealed class AesEncryptionServiceTests
{
    private readonly AesEncryptionService _service = new();

    private const string TestPassphrase = "Ma-Phrase-Secrete-De-Test-2024!";

    [Fact]
    public void EncryptAndDecrypt_ShouldReturnOriginalText()
    {
        // Arrange
        const string plainText = "Bonjour, ceci est un message confidentiel pour Kouroukan.";

        // Act
        var encrypted = _service.Encrypt(plainText, TestPassphrase);
        var decrypted = _service.Decrypt(encrypted, TestPassphrase);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_ShouldReturnBase64String()
    {
        // Act
        var encrypted = _service.Encrypt("Test", TestPassphrase);

        // Assert
        encrypted.Should().NotBeNullOrWhiteSpace();

        // Doit etre du Base64 valide
        var act = () => Convert.FromBase64String(encrypted);
        act.Should().NotThrow();
    }

    [Fact]
    public void Encrypt_SameTextTwice_ShouldProduceDifferentCiphertexts()
    {
        // Arrange
        const string plainText = "Meme texte, IVs differents.";

        // Act
        var encrypted1 = _service.Encrypt(plainText, TestPassphrase);
        var encrypted2 = _service.Encrypt(plainText, TestPassphrase);

        // Assert — les IV aleatoires garantissent des sorties differentes
        encrypted1.Should().NotBe(encrypted2, "chaque chiffrement doit utiliser un IV unique");

        // Mais les deux doivent dechiffrer vers le meme texte
        _service.Decrypt(encrypted1, TestPassphrase).Should().Be(plainText);
        _service.Decrypt(encrypted2, TestPassphrase).Should().Be(plainText);
    }

    [Fact]
    public void Decrypt_WrongPassphrase_ShouldThrowCryptographicException()
    {
        // Arrange
        const string plainText = "Message secret.";
        var encrypted = _service.Encrypt(plainText, TestPassphrase);

        // Act
        var act = () => _service.Decrypt(encrypted, "Mauvaise-Phrase-Secrete!");

        // Assert — AES-GCM detecte l'authentification echouee
        act.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void Decrypt_TamperedCiphertext_ShouldThrowCryptographicException()
    {
        // Arrange
        const string plainText = "Donnees sensibles.";
        var encrypted = _service.Encrypt(plainText, TestPassphrase);
        var bytes = Convert.FromBase64String(encrypted);

        // Alterer un octet au milieu du ciphertext (apres salt+nonce+tag = 44 octets)
        if (bytes.Length > 45)
        {
            bytes[45] = (byte)(bytes[45] ^ 0xFF);
        }

        var tamperedEncrypted = Convert.ToBase64String(bytes);

        // Act
        var act = () => _service.Decrypt(tamperedEncrypted, TestPassphrase);

        // Assert
        act.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void Decrypt_TruncatedCiphertext_ShouldThrowCryptographicException()
    {
        // Arrange — texte trop court (moins de salt + nonce + tag = 44 octets)
        var tooShort = Convert.ToBase64String(new byte[30]);

        // Act
        var act = () => _service.Decrypt(tooShort, TestPassphrase);

        // Assert
        act.Should().Throw<CryptographicException>()
            .WithMessage("*trop court*");
    }

    [Fact]
    public void Decrypt_InvalidBase64_ShouldThrowCryptographicException()
    {
        // Act
        var act = () => _service.Decrypt("ceci-n'est-pas-du-base64!!!", TestPassphrase);

        // Assert
        act.Should().Throw<CryptographicException>()
            .WithMessage("*Base64*");
    }

    [Fact]
    public void EncryptAndDecrypt_UnicodeText_ShouldPreserveContent()
    {
        // Arrange — texte avec des caracteres speciaux (accents, emojis, arabe)
        const string plainText = "Ecole de Conakry — Eleves: é à ü ö — مدرسة — 学校 — 🎓📚";

        // Act
        var encrypted = _service.Encrypt(plainText, TestPassphrase);
        var decrypted = _service.Decrypt(encrypted, TestPassphrase);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void EncryptAndDecrypt_EmptyContentAfterTrim_ShouldThrow()
    {
        // Act & Assert — texte vide
        var actEncrypt = () => _service.Encrypt("", TestPassphrase);
        actEncrypt.Should().Throw<ArgumentException>();

        var actDecrypt = () => _service.Decrypt("", TestPassphrase);
        actDecrypt.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EncryptAndDecrypt_LargeText_ShouldWork()
    {
        // Arrange — texte de 100 Ko
        var plainText = new string('A', 100_000);

        // Act
        var encrypted = _service.Encrypt(plainText, TestPassphrase);
        var decrypted = _service.Decrypt(encrypted, TestPassphrase);

        // Assert
        decrypted.Should().Be(plainText);
    }
}
