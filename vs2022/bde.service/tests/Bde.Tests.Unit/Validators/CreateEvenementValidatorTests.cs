using FluentAssertions;
using FluentValidation.TestHelper;
using Bde.Application.Commands;
using Bde.Application.Validators;
using Xunit;

namespace Bde.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateEvenementValidator.
/// </summary>
public sealed class CreateEvenementValidatorTests
{
    private readonly CreateEvenementValidator _validator;

    public CreateEvenementValidatorTests()
    {
        _validator = new CreateEvenementValidator();
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

    // ─── AssociationId ───

    [Fact]
    public void AssociationId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AssociationId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AssociationId);
    }

    // ─── DateEvenement ───

    [Fact]
    public void DateEvenement_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateEvenement = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateEvenement);
    }

    // ─── Lieu ───

    [Fact]
    public void Lieu_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Lieu = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Lieu);
    }

    [Fact]
    public void Lieu_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Lieu = new string('A', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Lieu);
    }

    // ─── StatutEvenement ───

    [Theory]
    [InlineData("Planifie")]
    [InlineData("Valide")]
    [InlineData("EnCours")]
    [InlineData("Termine")]
    [InlineData("Annule")]
    public void StatutEvenement_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutEvenement = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutEvenement);
    }

    [Fact]
    public void StatutEvenement_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutEvenement = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutEvenement);
    }

    [Fact]
    public void StatutEvenement_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutEvenement = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutEvenement);
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

    private static CreateEvenementCommand CreateValidCommand()
    {
        return new CreateEvenementCommand(
            TypeId: 1,
            Name: "Journee integration",
            Description: "Bienvenue",
            AssociationId: 5,
            DateEvenement: new DateTime(2025, 10, 15),
            Lieu: "Campus principal",
            Capacite: 200,
            TarifEntree: 5000m,
            NombreInscrits: 0,
            StatutEvenement: "Planifie",
            UserId: 1);
    }
}
