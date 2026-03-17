using FluentAssertions;
using GnValidation.Rules;

namespace GnValidation.Test.Rules;

/// <summary>
/// Tests unitaires pour <see cref="EmailValidator"/>.
/// </summary>
public sealed class EmailValidatorTests
{
    private readonly EmailValidator _validator = new();

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@domain.org")]
    [InlineData("admin@kouroukan.gn")]
    [InlineData("user+tag@example.co.uk")]
    public void IsValid_StandardEmail_ShouldReturnTrue(string email)
    {
        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeTrue("l'email {0} doit etre valide", email);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    [InlineData("user..name@domain.com")]
    [InlineData("user@domain")]
    public void IsValid_InvalidFormat_ShouldReturnFalse(string email)
    {
        // Act
        var result = _validator.IsValid(email);

        // Assert
        result.Should().BeFalse("l'email invalide {0} ne doit pas etre valide", email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_EmptyOrNull_ShouldReturnFalse(string? email)
    {
        // Act
        var result = _validator.IsValid(email!);

        // Assert
        result.Should().BeFalse("un email vide ou null ne doit pas etre valide");
    }
}
