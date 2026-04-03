using FluentAssertions;
using FluentValidation.TestHelper;
using Finances.Application.Commands;
using Finances.Application.Validators;
using Xunit;

namespace Finances.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdatePaiementValidator.
/// </summary>
public sealed class UpdatePaiementValidatorTests
{
    private readonly UpdatePaiementValidator _validator;

    public UpdatePaiementValidatorTests()
    {
        _validator = new UpdatePaiementValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- Id ---

    [Fact]
    public void Id_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Id = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Id_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { Id = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
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

    // --- MontantPaye ---

    [Fact]
    public void MontantPaye_Zero_Erreur()
    {
        var command = CreateValidCommand() with { MontantPaye = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MontantPaye);
    }

    // --- MoyenPaiement ---

    [Fact]
    public void MoyenPaiement_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { MoyenPaiement = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MoyenPaiement);
    }

    // --- StatutPaiement ---

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

    // --- UserId ---

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // --- Helper ---

    private static UpdatePaiementCommand CreateValidCommand()
    {
        return new UpdatePaiementCommand(
            Id: 1,
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
