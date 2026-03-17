using FluentAssertions;
using FluentValidation.TestHelper;
using GnValidation.Commands.Inscriptions;
using GnValidation.FluentValidation.Inscriptions;

namespace GnValidation.Test.FluentValidation;

/// <summary>
/// Tests unitaires pour <see cref="CreateInscriptionValidator"/>.
/// </summary>
public sealed class CreateInscriptionValidatorTests
{
    private readonly CreateInscriptionValidator _validator = new();

    private static CreateInscriptionCommand ValidCommand => new(
        EleveId: 1,
        ClasseId: 1,
        AnneeScolaireId: 1,
        DateInscription: new DateTime(2025, 9, 1),
        MontantInscription: 150000m,
        EstPaye: false,
        EstRedoublant: false,
        TypeEtablissement: "Public",
        SerieBac: null,
        StatutInscription: "EnAttente");

    [Fact]
    public void Validate_ValidInscription_ShouldHaveNoErrors()
    {
        // Act
        var result = _validator.TestValidate(ValidCommand);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ZeroEleveId_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { EleveId = 0 };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    [Fact]
    public void Validate_NegativeMontant_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { MontantInscription = -100m };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MontantInscription);
    }

    [Fact]
    public void Validate_ZeroMontant_ShouldNotHaveError()
    {
        // Arrange — gratuit (ecole publique primaire)
        var command = ValidCommand with { MontantInscription = 0m };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MontantInscription);
    }

    [Fact]
    public void Validate_InvalidStatut_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { StatutInscription = "Unknown" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StatutInscription);
    }

    [Theory]
    [InlineData("EnAttente")]
    [InlineData("Validee")]
    [InlineData("Annulee")]
    public void Validate_ValidStatut_ShouldNotHaveError(string statut)
    {
        // Arrange
        var command = ValidCommand with { StatutInscription = statut };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StatutInscription);
    }

    [Theory]
    [InlineData("SE")]
    [InlineData("SM")]
    [InlineData("SS")]
    [InlineData("FA")]
    public void Validate_ValidSerieBac_ShouldNotHaveError(string serie)
    {
        // Arrange
        var command = ValidCommand with { SerieBac = serie };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SerieBac);
    }

    [Fact]
    public void Validate_InvalidSerieBac_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { SerieBac = "XY" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SerieBac);
    }

    [Fact]
    public void Validate_NullSerieBac_ShouldNotHaveError()
    {
        // Arrange — la serie bac est optionnelle (pas pour les lyceens)
        var command = ValidCommand with { SerieBac = null };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SerieBac);
    }

    [Fact]
    public void Validate_InvalidTypeEtablissement_ShouldHaveError()
    {
        // Arrange
        var command = ValidCommand with { TypeEtablissement = "PriveMusulman" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TypeEtablissement);
    }
}
