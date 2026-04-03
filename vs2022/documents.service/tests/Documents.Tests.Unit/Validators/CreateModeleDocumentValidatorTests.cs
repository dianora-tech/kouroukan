using FluentAssertions;
using FluentValidation.TestHelper;
using Documents.Application.Commands;
using Documents.Application.Validators;
using Xunit;

namespace Documents.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateModeleDocumentValidator.
/// </summary>
public sealed class CreateModeleDocumentValidatorTests
{
    private readonly CreateModeleDocumentValidator _validator;

    public CreateModeleDocumentValidatorTests()
    {
        _validator = new CreateModeleDocumentValidator();
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

    // ─── Code ───

    [Fact]
    public void Code_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Code = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Code_Null_Erreur()
    {
        var command = CreateValidCommand() with { Code = null! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Code_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Code = new string('A', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Code_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Code = new string('A', 50) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    // ─── Contenu ───

    [Fact]
    public void Contenu_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Contenu = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    [Fact]
    public void Contenu_Null_Erreur()
    {
        var command = CreateValidCommand() with { Contenu = null! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    // ─── LogoUrl (optionnel) ───

    [Fact]
    public void LogoUrl_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { LogoUrl = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.LogoUrl);
    }

    // ─── CouleurPrimaire (optionnel, max 7) ───

    [Fact]
    public void CouleurPrimaire_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { CouleurPrimaire = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CouleurPrimaire);
    }

    [Fact]
    public void CouleurPrimaire_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { CouleurPrimaire = "#1234567" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CouleurPrimaire);
    }

    [Fact]
    public void CouleurPrimaire_Valide_PasDerreur()
    {
        var command = CreateValidCommand() with { CouleurPrimaire = "#16a34a" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CouleurPrimaire);
    }

    // ─── TextePiedPage (optionnel, max 500) ───

    [Fact]
    public void TextePiedPage_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { TextePiedPage = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TextePiedPage);
    }

    [Fact]
    public void TextePiedPage_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { TextePiedPage = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TextePiedPage);
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

    private static CreateModeleDocumentCommand CreateValidCommand()
    {
        return new CreateModeleDocumentCommand(
            TypeId: 1,
            Code: "BULL_NOTES",
            Contenu: "<html><body>{{nom}}</body></html>",
            LogoUrl: "https://example.com/logo.png",
            CouleurPrimaire: "#16a34a",
            TextePiedPage: "Pied de page",
            EstActif: true,
            UserId: 1);
    }
}
