using FluentAssertions;
using FluentValidation.TestHelper;
using Bde.Application.Commands;
using Bde.Application.Validators;
using Xunit;

namespace Bde.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateDepenseBdeValidator.
/// </summary>
public sealed class CreateDepenseBdeValidatorTests
{
    private readonly CreateDepenseBdeValidator _validator;

    public CreateDepenseBdeValidatorTests()
    {
        _validator = new CreateDepenseBdeValidator();
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

    // ─── Montant ───

    [Fact]
    public void Montant_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Montant = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Montant);
    }

    [Fact]
    public void Montant_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { Montant = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Montant);
    }

    [Fact]
    public void Montant_Positif_PasDerreur()
    {
        var command = CreateValidCommand() with { Montant = 1m };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Montant);
    }

    // ─── Motif ───

    [Fact]
    public void Motif_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Motif = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Motif);
    }

    [Fact]
    public void Motif_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Motif = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Motif);
    }

    // ─── Categorie ───

    [Theory]
    [InlineData("Materiel")]
    [InlineData("Location")]
    [InlineData("Prestataire")]
    [InlineData("Remboursement")]
    public void Categorie_Valide_PasDerreur(string categorie)
    {
        var command = CreateValidCommand() with { Categorie = categorie };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Categorie);
    }

    [Fact]
    public void Categorie_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Categorie = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Categorie);
    }

    [Fact]
    public void Categorie_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Categorie = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Categorie);
    }

    // ─── StatutValidation ───

    [Theory]
    [InlineData("Demandee")]
    [InlineData("ValideTresorier")]
    [InlineData("ValideSuper")]
    [InlineData("Refusee")]
    public void StatutValidation_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutValidation = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutValidation);
    }

    [Fact]
    public void StatutValidation_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutValidation = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutValidation);
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

    private static CreateDepenseBdeCommand CreateValidCommand()
    {
        return new CreateDepenseBdeCommand(
            TypeId: 1,
            Name: "Achat materiel",
            Description: "Fournitures",
            AssociationId: 5,
            Montant: 100000m,
            Motif: "Evenement de rentree",
            Categorie: "Materiel",
            StatutValidation: "Demandee",
            ValidateurId: 10,
            UserId: 1);
    }
}
