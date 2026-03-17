using FluentAssertions;
using FluentValidation.TestHelper;
using GnValidation.Commands.Inscriptions;
using GnValidation.FluentValidation.Inscriptions;

namespace GnValidation.Test.FluentValidation;

/// <summary>
/// Tests unitaires pour <see cref="CreateEleveValidator"/>.
/// </summary>
public sealed class CreateEleveValidatorTests
{
    private readonly CreateEleveValidator _validator = new();

    private static CreateEleveCommand ValidCommand => new(
        "Mamadou",
        "Diallo",
        new DateTime(2010, 5, 15),
        "Conakry",
        "M",
        "Guineenne",
        "Ratoma, Conakry",
        null,
        "MAT-2025-001",
        1,
        null,
        null,
        "Inscrit");

    [Fact]
    public void Validate_ValidEleve_ShouldHaveNoErrors()
    {
        // Act
        var result = _validator.TestValidate(ValidCommand);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyFirstName_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { FirstName = "" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_EmptyLastName_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { LastName = "" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_FutureDateNaissance_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { DateNaissance = DateTime.Today.AddDays(1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateNaissance)
            .WithErrorMessage("La date de naissance doit etre dans le passe");
    }

    [Theory]
    [InlineData("X")]
    [InlineData("Male")]
    [InlineData("")]
    public void Validate_InvalidGenre_ShouldHaveError(string genre)
    {
        // Arrange
        var command = ValidCommand with { Genre = genre };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Genre);
    }

    [Theory]
    [InlineData("M")]
    [InlineData("F")]
    public void Validate_ValidGenre_ShouldNotHaveError(string genre)
    {
        // Arrange
        var command = ValidCommand with { Genre = genre };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Genre);
    }

    [Fact]
    public void Validate_ZeroNiveauClasseId_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { NiveauClasseId = 0 };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NiveauClasseId);
    }

    [Fact]
    public void Validate_InvalidStatutInscription_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { StatutInscription = "Invalid" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StatutInscription);
    }

    [Fact]
    public void Validate_InvalidPhotoUrl_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { PhotoUrl = "not-a-url" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhotoUrl);
    }

    [Fact]
    public void Validate_ValidPhotoUrl_ShouldNotHaveError()
    {
        // Arrange
        var command = ValidCommand with { PhotoUrl = "https://storage.kouroukan.gn/photos/eleve-001.jpg" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhotoUrl);
    }

    [Fact]
    public void Validate_NullOptionalFields_ShouldNotHaveError()
    {
        // Arrange — tous les champs optionnels a null
        var command = ValidCommand with
        {
            Adresse = null,
            PhotoUrl = null,
            ClasseId = null,
            ParentId = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ZeroOptionalFk_ShouldHaveError()
    {
        // Arrange — ClasseId a 0 (doit etre > 0 quand present)
        var command = ValidCommand with { ClasseId = 0 };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClasseId);
    }
}
