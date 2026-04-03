using FluentAssertions;
using FluentValidation.TestHelper;
using Finances.Application.Commands;
using Finances.Application.Validators;
using Xunit;

namespace Finances.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreatePaiementValidator.
/// </summary>
public sealed class CreatePaiementValidatorTests
{
    private readonly CreatePaiementValidator _validator;

    public CreatePaiementValidatorTests()
    {
        _validator = new CreatePaiementValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- TypeId ---

    [Fact]
    public void TypeId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypeId);
    }

    // --- FactureId ---

    [Fact]
    public void FactureId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { FactureId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FactureId);
    }

    [Fact]
    public void FactureId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { FactureId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FactureId);
    }

    // --- MontantPaye ---

    [Fact]
    public void MontantPaye_Zero_Erreur()
    {
        var command = CreateValidCommand() with { MontantPaye = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MontantPaye);
    }

    [Fact]
    public void MontantPaye_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { MontantPaye = -100m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MontantPaye);
    }

    // --- DatePaiement ---

    [Fact]
    public void DatePaiement_Vide_Erreur()
    {
        var command = CreateValidCommand() with { DatePaiement = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DatePaiement);
    }

    // --- MoyenPaiement ---

    [Theory]
    [InlineData("OrangeMoney")]
    [InlineData("SoutraMoney")]
    [InlineData("MTNMoMo")]
    [InlineData("Especes")]
    public void MoyenPaiement_Valide_PasDerreur(string moyen)
    {
        var command = CreateValidCommand() with { MoyenPaiement = moyen };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MoyenPaiement);
    }

    [Fact]
    public void MoyenPaiement_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { MoyenPaiement = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MoyenPaiement);
    }

    [Fact]
    public void MoyenPaiement_Vide_Erreur()
    {
        var command = CreateValidCommand() with { MoyenPaiement = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MoyenPaiement);
    }

    // --- StatutPaiement ---

    [Theory]
    [InlineData("EnAttente")]
    [InlineData("Confirme")]
    [InlineData("Echec")]
    [InlineData("Rembourse")]
    public void StatutPaiement_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutPaiement = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutPaiement);
    }

    [Fact]
    public void StatutPaiement_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutPaiement = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutPaiement);
    }

    // --- NumeroRecu ---

    [Fact]
    public void NumeroRecu_Vide_Erreur()
    {
        var command = CreateValidCommand() with { NumeroRecu = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NumeroRecu);
    }

    [Fact]
    public void NumeroRecu_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { NumeroRecu = new string('R', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NumeroRecu);
    }

    // --- UserId ---

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // --- Helper ---

    private static CreatePaiementCommand CreateValidCommand()
    {
        return new CreatePaiementCommand(
            TypeId: 1,
            FactureId: 10,
            MontantPaye: 50000m,
            DatePaiement: DateTime.UtcNow,
            MoyenPaiement: "OrangeMoney",
            ReferenceMobileMoney: "REF-OM-001",
            StatutPaiement: "EnAttente",
            CaissierId: 3,
            NumeroRecu: "RECU-2024-001",
            UserId: 1);
    }
}
