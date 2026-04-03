using FluentAssertions;
using FluentValidation.TestHelper;
using Finances.Application.Commands;
using Finances.Application.Validators;
using Xunit;

namespace Finances.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateFactureValidator.
/// </summary>
public sealed class UpdateFactureValidatorTests
{
    private readonly UpdateFactureValidator _validator;

    public UpdateFactureValidatorTests()
    {
        _validator = new UpdateFactureValidator();
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

    // --- EleveId ---

    [Fact]
    public void EleveId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    // --- AnneeScolaireId ---

    [Fact]
    public void AnneeScolaireId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AnneeScolaireId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AnneeScolaireId);
    }

    // --- MontantTotal ---

    [Fact]
    public void MontantTotal_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { MontantTotal = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MontantTotal);
    }

    // --- DateEcheance ---

    [Fact]
    public void DateEcheance_AvantDateEmission_Erreur()
    {
        var command = CreateValidCommand() with
        {
            DateEmission = new DateTime(2024, 6, 15),
            DateEcheance = new DateTime(2024, 6, 1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateEcheance);
    }

    // --- StatutFacture ---

    [Fact]
    public void StatutFacture_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutFacture = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutFacture);
    }

    // --- NumeroFacture ---

    [Fact]
    public void NumeroFacture_Vide_Erreur()
    {
        var command = CreateValidCommand() with { NumeroFacture = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NumeroFacture);
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

    private static UpdateFactureCommand CreateValidCommand()
    {
        return new UpdateFactureCommand(
            Id: 1,
            TypeId: 1,
            EleveId: 10,
            ParentId: 5,
            AnneeScolaireId: 1,
            MontantTotal: 150000m,
            MontantPaye: 50000m,
            DateEmission: new DateTime(2024, 9, 1),
            DateEcheance: new DateTime(2024, 12, 31),
            StatutFacture: "Emise",
            NumeroFacture: "FACT-2024-001",
            UserId: 1);
    }
}
