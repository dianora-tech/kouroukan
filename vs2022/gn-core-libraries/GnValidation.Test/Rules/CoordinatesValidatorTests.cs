using FluentAssertions;
using GnValidation.Options;
using GnValidation.Rules;

namespace GnValidation.Test.Rules;

/// <summary>
/// Tests unitaires pour <see cref="CoordinatesValidator"/>.
/// </summary>
public sealed class CoordinatesValidatorTests
{
    private readonly CoordinatesValidator _validator;

    public CoordinatesValidatorTests()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new GnValidationOptions());
        _validator = new CoordinatesValidator(options);
    }

    [Fact]
    public void IsInCountry_Conakry_ShouldReturnTrue()
    {
        // Arrange — coordonnees de Conakry
        const double latitude = 9.6412;
        const double longitude = -13.5784;

        // Act
        var result = _validator.IsInCountry(latitude, longitude);

        // Assert
        result.Should().BeTrue("Conakry est en Guinee");
    }

    [Fact]
    public void IsInCountry_Kankan_ShouldReturnTrue()
    {
        // Arrange — coordonnees de Kankan
        const double latitude = 10.3854;
        const double longitude = -9.3055;

        // Act
        var result = _validator.IsInCountry(latitude, longitude);

        // Assert
        result.Should().BeTrue("Kankan est en Guinee");
    }

    [Fact]
    public void IsInCountry_Paris_ShouldReturnFalse()
    {
        // Arrange — coordonnees de Paris
        const double latitude = 48.8566;
        const double longitude = 2.3522;

        // Act
        var result = _validator.IsInCountry(latitude, longitude);

        // Assert
        result.Should().BeFalse("Paris n'est pas en Guinee");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(45.5)]
    [InlineData(-90)]
    [InlineData(90)]
    public void IsValidLatitude_ValidRange_ShouldReturnTrue(double latitude)
    {
        // Act
        var result = _validator.IsValidLatitude(latitude);

        // Assert
        result.Should().BeTrue("la latitude {0} est dans la plage valide", latitude);
    }

    [Theory]
    [InlineData(91)]
    [InlineData(-91)]
    [InlineData(200)]
    public void IsValidLatitude_OutOfRange_ShouldReturnFalse(double latitude)
    {
        // Act
        var result = _validator.IsValidLatitude(latitude);

        // Assert
        result.Should().BeFalse("la latitude {0} est hors plage", latitude);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(180)]
    [InlineData(-180)]
    public void IsValidLongitude_ValidRange_ShouldReturnTrue(double longitude)
    {
        // Act
        var result = _validator.IsValidLongitude(longitude);

        // Assert
        result.Should().BeTrue("la longitude {0} est dans la plage valide", longitude);
    }

    [Theory]
    [InlineData(181)]
    [InlineData(-181)]
    public void IsValidLongitude_OutOfRange_ShouldReturnFalse(double longitude)
    {
        // Act
        var result = _validator.IsValidLongitude(longitude);

        // Assert
        result.Should().BeFalse("la longitude {0} est hors plage", longitude);
    }

    [Fact]
    public void IsInCountry_InvalidLatitude_ShouldReturnFalse()
    {
        // Act
        var result = _validator.IsInCountry(91, -13);

        // Assert
        result.Should().BeFalse("une latitude invalide doit retourner false");
    }
}
