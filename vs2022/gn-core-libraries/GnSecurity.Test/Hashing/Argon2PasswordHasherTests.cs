using FluentAssertions;
using GnSecurity.Hashing;

namespace GnSecurity.Test.Hashing;

/// <summary>
/// Tests unitaires pour <see cref="Argon2PasswordHasher"/>.
/// </summary>
public sealed class Argon2PasswordHasherTests
{
    private readonly Argon2PasswordHasher _hasher = new();

    [Fact]
    public async Task HashAsync_ShouldReturnSaltColonHashFormat()
    {
        // Arrange
        const string password = "MonMotDePasse!123";

        // Act
        var result = await _hasher.HashAsync(password);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain(":");

        var parts = result.Split(':');
        parts.Should().HaveCount(2);

        // Les deux parties doivent etre du Base64 valide
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        salt.Should().HaveCount(16, "le sel doit faire 16 octets (128 bits)");
        hash.Should().HaveCount(32, "le hash doit faire 32 octets (256 bits)");
    }

    [Fact]
    public async Task VerifyAsync_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        const string password = "MotDePasseCorrect$456";
        var hashedPassword = await _hasher.HashAsync(password);

        // Act
        var result = await _hasher.VerifyAsync(password, hashedPassword);

        // Assert
        result.Should().BeTrue("le mot de passe correct doit etre valide");
    }

    [Fact]
    public async Task VerifyAsync_WrongPassword_ShouldReturnFalse()
    {
        // Arrange
        const string password = "MotDePasseOriginal!789";
        const string wrongPassword = "MauvaisMotDePasse!000";
        var hashedPassword = await _hasher.HashAsync(password);

        // Act
        var result = await _hasher.VerifyAsync(wrongPassword, hashedPassword);

        // Assert
        result.Should().BeFalse("un mot de passe incorrect ne doit pas etre valide");
    }

    [Fact]
    public async Task HashAsync_SamePassword_ShouldProduceDifferentSalts()
    {
        // Arrange
        const string password = "MemeMotDePasse!ABC";

        // Act
        var hash1 = await _hasher.HashAsync(password);
        var hash2 = await _hasher.HashAsync(password);

        // Assert
        hash1.Should().NotBe(hash2, "chaque hash doit avoir un sel unique aleatoire");

        var salt1 = hash1.Split(':')[0];
        var salt2 = hash2.Split(':')[0];
        salt1.Should().NotBe(salt2, "les sels doivent etre differents");
    }

    [Fact]
    public async Task VerifyAsync_TamperedHash_ShouldReturnFalse()
    {
        // Arrange
        const string password = "SecurePassword!999";
        var hashedPassword = await _hasher.HashAsync(password);

        // Alterer le hash (changer un caractere dans la partie hash)
        var parts = hashedPassword.Split(':');
        var hashBytes = Convert.FromBase64String(parts[1]);
        hashBytes[0] = (byte)(hashBytes[0] ^ 0xFF); // Inverser le premier octet
        var tamperedHash = $"{parts[0]}:{Convert.ToBase64String(hashBytes)}";

        // Act
        var result = await _hasher.VerifyAsync(password, tamperedHash);

        // Assert
        result.Should().BeFalse("un hash altere ne doit pas etre valide");
    }

    [Fact]
    public async Task VerifyAsync_InvalidFormat_ShouldReturnFalse()
    {
        // Arrange
        const string password = "Test!123";

        // Act & Assert — pas de separateur ':'
        var result1 = await _hasher.VerifyAsync(password, "invalidformat");
        result1.Should().BeFalse();

        // Act & Assert — Base64 invalide
        var result2 = await _hasher.VerifyAsync(password, "not-base64:also-not-base64!");
        result2.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HashAsync_InvalidPassword_ShouldThrow(string? password)
    {
        // Act
        var act = () => _hasher.HashAsync(password!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task VerifyAsync_InvalidPassword_ShouldThrow(string? password)
    {
        // Act
        var act = () => _hasher.VerifyAsync(password!, "c29tZXNhbHQ=:c29tZWhhc2g=");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
