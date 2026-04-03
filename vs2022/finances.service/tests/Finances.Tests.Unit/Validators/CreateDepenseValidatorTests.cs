using FluentAssertions;
using FluentValidation.TestHelper;
using Finances.Application.Commands;
using Finances.Application.Validators;
using Xunit;

namespace Finances.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateDepenseValidator.
/// </summary>
public sealed class CreateDepenseValidatorTests
{
    private readonly CreateDepenseValidator _validator;

    public CreateDepenseValidatorTests()
    {
        _validator = new CreateDepenseValidator();
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

    [Fact]
    public void TypeId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypeId);
    }

    // --- Montant ---

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
        var command = CreateValidCommand() with { Montant = -100m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Montant);
    }

    // --- MotifDepense ---

    [Fact]
    public void MotifDepense_Vide_Erreur()
    {
        var command = CreateValidCommand() with { MotifDepense = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MotifDepense);
    }

    [Fact]
    public void MotifDepense_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { MotifDepense = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MotifDepense);
    }

    // --- Categorie ---

    [Theory]
    [InlineData("Personnel")]
    [InlineData("Fournitures")]
    [InlineData("Maintenance")]
    [InlineData("Evenements")]
    [InlineData("BDE")]
    [InlineData("Equipements")]
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

    // --- BeneficiaireNom ---

    [Fact]
    public void BeneficiaireNom_Vide_Erreur()
    {
        var command = CreateValidCommand() with { BeneficiaireNom = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BeneficiaireNom);
    }

    [Fact]
    public void BeneficiaireNom_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { BeneficiaireNom = new string('N', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BeneficiaireNom);
    }

    // --- StatutDepense ---

    [Theory]
    [InlineData("Demande")]
    [InlineData("ValideN1")]
    [InlineData("ValideFinance")]
    [InlineData("ValideDirection")]
    [InlineData("Executee")]
    [InlineData("Archivee")]
    public void StatutDepense_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutDepense = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutDepense);
    }

    [Fact]
    public void StatutDepense_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutDepense = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutDepense);
    }

    // --- DateDemande ---

    [Fact]
    public void DateDemande_Vide_Erreur()
    {
        var command = CreateValidCommand() with { DateDemande = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateDemande);
    }

    // --- NumeroJustificatif ---

    [Fact]
    public void NumeroJustificatif_Vide_Erreur()
    {
        var command = CreateValidCommand() with { NumeroJustificatif = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NumeroJustificatif);
    }

    [Fact]
    public void NumeroJustificatif_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { NumeroJustificatif = new string('J', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NumeroJustificatif);
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

    private static CreateDepenseCommand CreateValidCommand()
    {
        return new CreateDepenseCommand(
            TypeId: 1,
            Montant: 50000m,
            MotifDepense: "Achat fournitures bureau",
            Categorie: "Fournitures",
            BeneficiaireNom: "Papeterie Centrale",
            BeneficiaireTelephone: "622000000",
            BeneficiaireNIF: "NIF-001",
            StatutDepense: "Demande",
            DateDemande: DateTime.UtcNow,
            DateValidation: null,
            ValidateurId: null,
            PieceJointeUrl: null,
            NumeroJustificatif: "JUST-2024-001",
            UserId: 1);
    }
}
