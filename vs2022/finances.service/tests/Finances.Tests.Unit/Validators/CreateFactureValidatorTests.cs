using FluentAssertions;
using FluentValidation.TestHelper;
using Finances.Application.Commands;
using Finances.Application.Validators;
using Xunit;

namespace Finances.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateFactureValidator.
/// </summary>
public sealed class CreateFactureValidatorTests
{
    private readonly CreateFactureValidator _validator;

    public CreateFactureValidatorTests()
    {
        _validator = new CreateFactureValidator();
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

    [Fact]
    public void MontantTotal_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { MontantTotal = 0m, MontantPaye = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MontantTotal);
    }

    // --- MontantPaye ---

    [Fact]
    public void MontantPaye_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { MontantPaye = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MontantPaye);
    }

    // --- DateEmission ---

    [Fact]
    public void DateEmission_Vide_Erreur()
    {
        var command = CreateValidCommand() with { DateEmission = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateEmission);
    }

    // --- DateEcheance ---

    [Fact]
    public void DateEcheance_Vide_Erreur()
    {
        var command = CreateValidCommand() with { DateEcheance = default, DateEmission = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateEcheance);
    }

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

    [Fact]
    public void DateEcheance_EgaleDateEmission_PasDerreur()
    {
        var date = new DateTime(2024, 6, 15);
        var command = CreateValidCommand() with { DateEmission = date, DateEcheance = date };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.DateEcheance);
    }

    // --- StatutFacture ---

    [Theory]
    [InlineData("Emise")]
    [InlineData("PartPaye")]
    [InlineData("Payee")]
    [InlineData("Echue")]
    [InlineData("Annulee")]
    public void StatutFacture_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutFacture = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutFacture);
    }

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

    [Fact]
    public void NumeroFacture_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { NumeroFacture = new string('F', 51) };

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

    private static CreateFactureCommand CreateValidCommand()
    {
        return new CreateFactureCommand(
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
