using FluentAssertions;
using FluentValidation.TestHelper;
using Bde.Application.Commands;
using Bde.Application.Validators;
using Xunit;

namespace Bde.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateAssociationValidator.
/// </summary>
public sealed class UpdateAssociationValidatorTests
{
    private readonly UpdateAssociationValidator _validator;

    public UpdateAssociationValidatorTests()
    {
        _validator = new UpdateAssociationValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ─── Id ───

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

    // ─── TypeId ───

    [Fact]
    public void TypeId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = 0 };

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

    // ─── Statut ───

    [Fact]
    public void Statut_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Statut = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Statut);
    }

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

    // ─── BudgetAnnuel ───

    [Fact]
    public void BudgetAnnuel_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { BudgetAnnuel = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BudgetAnnuel);
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

    private static UpdateAssociationCommand CreateValidCommand()
    {
        return new UpdateAssociationCommand(
            Id: 1,
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
