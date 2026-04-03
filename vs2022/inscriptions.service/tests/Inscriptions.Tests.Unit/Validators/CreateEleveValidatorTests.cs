using FluentAssertions;
using FluentValidation.TestHelper;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Validators;
using Xunit;

namespace Inscriptions.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateEleveValidator.
/// </summary>
public sealed class CreateEleveValidatorTests
{
    private readonly CreateEleveValidator _validator;

    public CreateEleveValidatorTests()
    {
        _validator = new CreateEleveValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ─── FirstName ───

    [Fact]
    public void FirstName_Vide_Erreur()
    {
        var command = CreateValidCommand() with { FirstName = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void FirstName_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { FirstName = new string('A', 101) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    // ─── LastName ───

    [Fact]
    public void LastName_Vide_Erreur()
    {
        var command = CreateValidCommand() with { LastName = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void LastName_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { LastName = new string('A', 101) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    // ─── DateNaissance ───

    [Fact]
    public void DateNaissance_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateNaissance = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateNaissance);
    }

    // ─── LieuNaissance ───

    [Fact]
    public void LieuNaissance_Vide_Erreur()
    {
        var command = CreateValidCommand() with { LieuNaissance = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LieuNaissance);
    }

    [Fact]
    public void LieuNaissance_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { LieuNaissance = new string('A', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LieuNaissance);
    }

    // ─── Genre ───

    [Theory]
    [InlineData("M")]
    [InlineData("F")]
    public void Genre_Valide_PasDerreur(string genre)
    {
        var command = CreateValidCommand() with { Genre = genre };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Genre);
    }

    [Fact]
    public void Genre_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Genre = "X" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Genre);
    }

    [Fact]
    public void Genre_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Genre = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Genre);
    }

    // ─── Nationalite ───

    [Fact]
    public void Nationalite_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Nationalite = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Nationalite);
    }

    [Fact]
    public void Nationalite_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Nationalite = new string('A', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Nationalite);
    }

    // ─── Adresse (optionnel) ───

    [Fact]
    public void Adresse_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Adresse = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Adresse);
    }

    [Fact]
    public void Adresse_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Adresse = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Adresse);
    }

    // ─── NumeroMatricule ───

    [Fact]
    public void NumeroMatricule_Vide_Erreur()
    {
        var command = CreateValidCommand() with { NumeroMatricule = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NumeroMatricule);
    }

    [Fact]
    public void NumeroMatricule_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { NumeroMatricule = new string('A', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NumeroMatricule);
    }

    // ─── NiveauClasseId ───

    [Fact]
    public void NiveauClasseId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { NiveauClasseId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NiveauClasseId);
    }

    [Fact]
    public void NiveauClasseId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { NiveauClasseId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NiveauClasseId);
    }

    // ─── StatutInscription ───

    [Theory]
    [InlineData("Prospect")]
    [InlineData("PreInscrit")]
    [InlineData("Inscrit")]
    [InlineData("Radie")]
    public void StatutInscription_TousLesStatutsValides(string statut)
    {
        var command = CreateValidCommand() with { StatutInscription = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutInscription);
    }

    [Fact]
    public void StatutInscription_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutInscription = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutInscription);
    }

    [Fact]
    public void StatutInscription_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutInscription = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutInscription);
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

    private static CreateEleveCommand CreateValidCommand()
    {
        return new CreateEleveCommand(
            FirstName: "Mamadou",
            LastName: "Diallo",
            DateNaissance: new DateTime(2010, 5, 15),
            LieuNaissance: "Conakry",
            Genre: "M",
            Nationalite: "Guineenne",
            Adresse: "Kaloum",
            PhotoUrl: null,
            NumeroMatricule: "MAT-001",
            NiveauClasseId: 1,
            ClasseId: null,
            ParentId: null,
            StatutInscription: "Inscrit",
            UserId: 1);
    }
}
