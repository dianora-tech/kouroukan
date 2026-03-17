using FluentAssertions;
using GnValidation.Options;
using GnValidation.Rules;
using Microsoft.Extensions.Options;

namespace GnValidation.Test.Rules;

/// <summary>
/// Tests unitaires pour <see cref="PhoneNumberValidator"/>.
/// </summary>
public sealed class PhoneNumberValidatorTests
{
    private readonly PhoneNumberValidator _validator;

    public PhoneNumberValidatorTests()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new GnValidationOptions
        {
            DefaultRegionCode = "GN"
        });
        _validator = new PhoneNumberValidator(options);
    }

    [Theory]
    [InlineData("+224621000000")]
    [InlineData("+224622123456")]
    [InlineData("621000000")]
    [InlineData("622123456")]
    public void IsValid_GuineanMobile_ShouldReturnTrue(string phoneNumber)
    {
        // Act
        var result = _validator.IsValid(phoneNumber);

        // Assert
        result.Should().BeTrue("le numero guineen {0} doit etre valide", phoneNumber);
    }

    [Theory]
    [InlineData("+33612345678")]
    [InlineData("+221771234567")]
    public void IsValid_InternationalFormat_ShouldReturnTrue(string phoneNumber)
    {
        // Act
        var result = _validator.IsValid(phoneNumber);

        // Assert
        result.Should().BeTrue("le numero international {0} doit etre valide", phoneNumber);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("+224000")]
    [InlineData("000000000")]
    public void IsValid_InvalidNumber_ShouldReturnFalse(string phoneNumber)
    {
        // Act
        var result = _validator.IsValid(phoneNumber);

        // Assert
        result.Should().BeFalse("le numero invalide {0} ne doit pas etre valide", phoneNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_EmptyOrNull_ShouldReturnFalse(string? phoneNumber)
    {
        // Act
        var result = _validator.IsValid(phoneNumber!);

        // Assert
        result.Should().BeFalse("un numero vide ou null ne doit pas etre valide");
    }

    [Fact]
    public void Format_GuineanMobile_ShouldReturnE164()
    {
        // Arrange
        const string phoneNumber = "621000000";

        // Act
        var result = _validator.Format(phoneNumber);

        // Assert
        result.Should().NotBeNull();
        result.Should().StartWith("+224", "le format E.164 doit commencer par l'indicatif guineen");
    }

    [Fact]
    public void Format_InvalidNumber_ShouldReturnNull()
    {
        // Act
        var result = _validator.Format("abc");

        // Assert
        result.Should().BeNull("un numero invalide ne peut pas etre formate");
    }

    [Fact]
    public void GetRegionCode_GuineanNumber_ShouldReturnGN()
    {
        // Arrange
        const string phoneNumber = "+224621000000";

        // Act
        var result = _validator.GetRegionCode(phoneNumber);

        // Assert
        result.Should().Be("GN", "le code region d'un numero guineen doit etre GN");
    }

    [Fact]
    public void GetRegionCode_FrenchNumber_ShouldReturnFR()
    {
        // Arrange
        const string phoneNumber = "+33612345678";

        // Act
        var result = _validator.GetRegionCode(phoneNumber);

        // Assert
        result.Should().Be("FR", "le code region d'un numero francais doit etre FR");
    }

    [Fact]
    public void IsValid_WithExplicitRegionCode_ShouldUseSpecifiedRegion()
    {
        // Arrange — numero francais valide
        const string phoneNumber = "0612345678";

        // Act
        var result = _validator.IsValid(phoneNumber, "FR");

        // Assert
        result.Should().BeTrue("le numero francais doit etre valide avec le code region FR");
    }
}
