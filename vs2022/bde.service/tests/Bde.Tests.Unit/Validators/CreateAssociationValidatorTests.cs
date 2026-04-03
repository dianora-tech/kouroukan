using FluentAssertions;
using FluentValidation.TestHelper;
using Bde.Application.Commands;
using Bde.Application.Validators;
using Xunit;

namespace Bde.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateAssociationValidator.
/// </summary>
public sealed class CreateAssociationValidatorTests
{
    private readonly CreateAssociationValidator _validator;

    public CreateAssociationValidatorTests()
    {
        _validator = new CreateAssociationValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ─── TypeId ───

    [Fact]
    public void TypeId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypeId);
    }

    [Fact]
    public void TypeId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypeId);
    }

    // ─── Name ───

    [Fact]
    public void Name_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Name = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Name = new string('A', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_200Caracteres_PasDerreur()
    {
        var command = CreateValidCommand() with { Name = new string('A', 200) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    // ─── Description (optionnel) ───

    [Fact]
    public void Description_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Description = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_TropLongue_Erreur()
    {
        var command = CreateValidCommand() with { Description = new string('A', 1001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    // ─── Sigle (optionnel) ───

    [Fact]
    public void Sigle_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Sigle = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Sigle);
    }

    [Fact]
    public void Sigle_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Sigle = new string('A', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Sigle);
    }

    // ─── AnneeScolaire ───

    [Fact]
    public void AnneeScolaire_Vide_Erreur()
    {
        var command = CreateValidCommand() with { AnneeScolaire = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AnneeScolaire);
    }

    [Fact]
    public void AnneeScolaire_TropLongue_Erreur()
    {
        var command = CreateValidCommand() with { AnneeScolaire = new string('A', 21) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AnneeScolaire);
    }

    // ─── Statut ───

    [Theory]
    [InlineData("Active")]
    [InlineData("Suspendue")]
    [InlineData("Dissoute")]
    public void Statut_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { Statut = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Statut);
    }

    [Fact]
    public void Statut_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Statut = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Statut);
    }

    [Fact]
    public void Statut_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Statut = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Statut);
    }

    // ─── BudgetAnnuel ───

    [Fact]
    public void BudgetAnnuel_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { BudgetAnnuel = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BudgetAnnuel);
    }

    [Fact]
    public void BudgetAnnuel_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { BudgetAnnuel = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.BudgetAnnuel);
    }

    // ─── UserId ───

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ─── Helper ───

    private static CreateAssociationCommand CreateValidCommand()
    {
        return new CreateAssociationCommand(
            TypeId: 1,
            Name: "BDE Informatique",
            Description: "Association etudiante",
            Sigle: "BDE",
            AnneeScolaire: "2025-2026",
            Statut: "Active",
            BudgetAnnuel: 500000m,
            SuperviseurId: 10,
            UserId: 1);
    }
}
