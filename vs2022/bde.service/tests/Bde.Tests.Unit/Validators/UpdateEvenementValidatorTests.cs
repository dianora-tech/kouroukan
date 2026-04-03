using FluentAssertions;
using FluentValidation.TestHelper;
using Bde.Application.Commands;
using Bde.Application.Validators;
using Xunit;

namespace Bde.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateEvenementValidator.
/// </summary>
public sealed class UpdateEvenementValidatorTests
{
    private readonly UpdateEvenementValidator _validator;

    public UpdateEvenementValidatorTests()
    {
        _validator = new UpdateEvenementValidator();
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

    // ─── StatutEvenement ───

    [Fact]
    public void StatutEvenement_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutEvenement = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutEvenement);
    }

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

    // ─── UserId ───

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ─── Helper ───

    private static UpdateEvenementCommand CreateValidCommand()
    {
        return new UpdateEvenementCommand(
            Id: 1,
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
