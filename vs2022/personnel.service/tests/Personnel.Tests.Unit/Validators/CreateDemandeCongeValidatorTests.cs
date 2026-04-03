using FluentAssertions;
using FluentValidation.TestHelper;
using Personnel.Application.Commands;
using Personnel.Application.Validators;
using Xunit;

namespace Personnel.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateDemandeCongeValidator.
/// </summary>
public sealed class CreateDemandeCongeValidatorTests
{
    private readonly CreateDemandeCongeValidator _validator;

    public CreateDemandeCongeValidatorTests()
    {
        _validator = new CreateDemandeCongeValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
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
    public void Name_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Name = new string('A', 200) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    // ─── Description ───

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
        var command = CreateValidCommand() with { Description = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    // ─── EnseignantId ───

    [Fact]
    public void EnseignantId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EnseignantId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EnseignantId);
    }

    [Fact]
    public void EnseignantId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { EnseignantId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EnseignantId);
    }

    // ─── DateDebut ───

    [Fact]
    public void DateDebut_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateDebut = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateDebut);
    }

    // ─── DateFin ───

    [Fact]
    public void DateFin_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateFin = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateFin);
    }

    [Fact]
    public void DateFin_AnterieureDateDebut_Erreur()
    {
        var command = CreateValidCommand() with
        {
            DateDebut = new DateTime(2025, 7, 15),
            DateFin = new DateTime(2025, 7, 1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateFin);
    }

    [Fact]
    public void DateFin_EgaleDateDebut_Erreur()
    {
        var sameDate = new DateTime(2025, 7, 1);
        var command = CreateValidCommand() with
        {
            DateDebut = sameDate,
            DateFin = sameDate
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateFin);
    }

    [Fact]
    public void DateFin_PosterieureDateDebut_PasDerreur()
    {
        var command = CreateValidCommand() with
        {
            DateDebut = new DateTime(2025, 7, 1),
            DateFin = new DateTime(2025, 7, 15)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.DateFin);
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
        var command = CreateValidCommand() with { Motif = new string('M', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Motif);
    }

    // ─── StatutDemande ───

    [Theory]
    [InlineData("Soumise")]
    [InlineData("ApprouveeN1")]
    [InlineData("ApprouveeDirection")]
    [InlineData("Refusee")]
    public void StatutDemande_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutDemande = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutDemande);
    }

    [Fact]
    public void StatutDemande_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutDemande = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutDemande);
    }

    [Fact]
    public void StatutDemande_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutDemande = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutDemande);
    }

    // ─── PieceJointeUrl ───

    [Fact]
    public void PieceJointeUrl_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PieceJointeUrl);
    }

    [Fact]
    public void PieceJointeUrl_UrlValide_PasDerreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = "https://docs.example.com/certificat.pdf" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PieceJointeUrl);
    }

    [Fact]
    public void PieceJointeUrl_UrlInvalide_Erreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = "pas-une-url" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PieceJointeUrl);
    }

    // ─── CommentaireValidateur ───

    [Fact]
    public void CommentaireValidateur_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { CommentaireValidateur = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CommentaireValidateur);
    }

    [Fact]
    public void CommentaireValidateur_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { CommentaireValidateur = new string('C', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CommentaireValidateur);
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

    // ─── UserId ───

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void UserId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { UserId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ─── Helper ───

    private static CreateDemandeCongeCommand CreateValidCommand()
    {
        return new CreateDemandeCongeCommand(
            Name: "Conge annuel",
            Description: "Conge de fin d'annee",
            EnseignantId: 10,
            DateDebut: new DateTime(2025, 7, 1),
            DateFin: new DateTime(2025, 7, 15),
            Motif: "Repos annuel",
            StatutDemande: "Soumise",
            PieceJointeUrl: null,
            CommentaireValidateur: null,
            ValidateurId: null,
            DateValidation: null,
            ImpactPaie: false,
            TypeId: 1,
            UserId: 1);
    }
}
