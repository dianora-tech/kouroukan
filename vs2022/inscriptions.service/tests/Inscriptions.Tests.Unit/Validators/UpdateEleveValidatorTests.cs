using FluentAssertions;
using FluentValidation.TestHelper;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Validators;
using Xunit;

namespace Inscriptions.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateEleveValidator.
/// </summary>
public sealed class UpdateEleveValidatorTests
{
    private readonly UpdateEleveValidator _validator;

    public UpdateEleveValidatorTests()
    {
        _validator = new UpdateEleveValidator();
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

    // ─── DateNaissance ───

    [Fact]
    public void DateNaissance_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateNaissance = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateNaissance);
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

    // ─── NiveauClasseId ───

    [Fact]
    public void NiveauClasseId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { NiveauClasseId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NiveauClasseId);
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

    private static UpdateEleveCommand CreateValidCommand()
    {
        return new UpdateEleveCommand(
            Id: 1,
            FirstName: "Mamadou",
            LastName: "Diallo",
            DateNaissance: new DateTime(2010, 5, 15),
            LieuNaissance: "Conakry",
            Genre: "M",
            Nationalite: "Guineenne",
            Adresse: null,
            PhotoUrl: null,
            NumeroMatricule: "MAT-001",
            NiveauClasseId: 1,
            ClasseId: null,
            ParentId: null,
            StatutInscription: "Inscrit",
            UserId: 1);
    }
}
