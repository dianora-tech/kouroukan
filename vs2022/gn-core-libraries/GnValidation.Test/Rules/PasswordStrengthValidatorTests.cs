using FluentAssertions;
using GnValidation.Options;
using GnValidation.Rules;

namespace GnValidation.Test.Rules;

/// <summary>
/// Tests unitaires pour <see cref="PasswordStrengthValidator"/>.
/// </summary>
public sealed class PasswordStrengthValidatorTests
{
    private readonly PasswordStrengthValidator _validator;

    public PasswordStrengthValidatorTests()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new GnValidationOptions());
        _validator = new PasswordStrengthValidator(options);
    }

    [Fact]
    public void Validate_StrongPassword_ShouldBeValid()
    {
        // Arrange
        const string password = "MonPass@123";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeTrue("un mot de passe fort doit etre valide");
        result.Score.Should().Be(5, "tous les criteres sont respectes");
        result.Suggestions.Should().BeEmpty("aucune suggestion pour un mot de passe fort");
    }

    [Fact]
    public void Validate_TooShort_ShouldBeInvalid()
    {
        // Arrange
        const string password = "Ab1!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse("un mot de passe trop court ne doit pas etre valide");
        result.Score.Should().Be(4, "4 criteres sur 5 sont respectes (tout sauf longueur)");
    }

    [Fact]
    public void Validate_NoUppercase_ShouldBeInvalid()
    {
        // Arrange
        const string password = "monpass@123";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse("un mot de passe sans majuscule ne doit pas etre valide");
        result.Suggestions.Should().Contain(s => s.Contains("majuscule"));
    }

    [Fact]
    public void Validate_NoLowercase_ShouldBeInvalid()
    {
        // Arrange
        const string password = "MONPASS@123";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse("un mot de passe sans minuscule ne doit pas etre valide");
        result.Suggestions.Should().Contain(s => s.Contains("minuscule"));
    }

    [Fact]
    public void Validate_NoDigit_ShouldBeInvalid()
    {
        // Arrange
        const string password = "MonPass@abc";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse("un mot de passe sans chiffre ne doit pas etre valide");
        result.Suggestions.Should().Contain(s => s.Contains("chiffre"));
    }

    [Fact]
    public void Validate_NoSpecialChar_ShouldBeInvalid()
    {
        // Arrange
        const string password = "MonPass1234";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse("un mot de passe sans caractere special ne doit pas etre valide");
        result.Suggestions.Should().Contain(s => s.Contains("special"));
    }

    [Fact]
    public void Validate_EmptyPassword_ShouldBeInvalid()
    {
        // Act
        var result = _validator.Validate("");

        // Assert
        result.IsValid.Should().BeFalse("un mot de passe vide ne doit pas etre valide");
        result.Score.Should().Be(0, "aucun critere n'est respecte");
        result.Suggestions.Should().HaveCount(5, "toutes les suggestions doivent etre presentes");
    }

    [Fact]
    public void Validate_NullPassword_ShouldBeInvalid()
    {
        // Act
        var result = _validator.Validate(null!);

        // Assert
        result.IsValid.Should().BeFalse("un mot de passe null ne doit pas etre valide");
        result.Score.Should().Be(0);
    }

    [Fact]
    public void Validate_Suggestions_ShouldBeInFrench()
    {
        // Arrange — mot de passe avec seulement des lettres minuscules
        const string password = "ab";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.Suggestions.Should().AllSatisfy(s =>
        {
            s.Should().NotBeNullOrWhiteSpace();
            // Verifier que les suggestions ne sont pas en anglais
            s.Should().NotContainAny("must", "should", "required", "at least");
        });
    }
}
