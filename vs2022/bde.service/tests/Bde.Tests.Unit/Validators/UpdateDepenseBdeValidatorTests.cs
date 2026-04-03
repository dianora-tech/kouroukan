using FluentAssertions;
using FluentValidation.TestHelper;
using Bde.Application.Commands;
using Bde.Application.Validators;
using Xunit;

namespace Bde.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateDepenseBdeValidator.
/// </summary>
public sealed class UpdateDepenseBdeValidatorTests
{
    private readonly UpdateDepenseBdeValidator _validator;

    public UpdateDepenseBdeValidatorTests()
    {
        _validator = new UpdateDepenseBdeValidator();
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

    // ─── Motif ───

    [Fact]
    public void Motif_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Motif = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Motif);
    }

    // ─── Categorie ───

    [Fact]
    public void Categorie_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Categorie = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Categorie);
    }

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

    // ─── StatutValidation ───

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

    private static UpdateDepenseBdeCommand CreateValidCommand()
    {
        return new UpdateDepenseBdeCommand(
            Id: 1,
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
