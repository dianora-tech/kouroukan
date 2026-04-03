using FluentAssertions;
using FluentValidation.TestHelper;
using Documents.Application.Commands;
using Documents.Application.Validators;
using Xunit;

namespace Documents.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateModeleDocumentValidator.
/// </summary>
public sealed class UpdateModeleDocumentValidatorTests
{
    private readonly UpdateModeleDocumentValidator _validator;

    public UpdateModeleDocumentValidatorTests()
    {
        _validator = new UpdateModeleDocumentValidator();
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

    // ─── Code ───

    [Fact]
    public void Code_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Code = "" };

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

    // ─── Contenu ───

    [Fact]
    public void Contenu_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Contenu = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    // ─── CouleurPrimaire ───

    [Fact]
    public void CouleurPrimaire_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { CouleurPrimaire = "#1234567" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CouleurPrimaire);
    }

    [Fact]
    public void CouleurPrimaire_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { CouleurPrimaire = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CouleurPrimaire);
    }

    // ─── TextePiedPage ───

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

    // ─── Helper ───

    private static UpdateModeleDocumentCommand CreateValidCommand()
    {
        return new UpdateModeleDocumentCommand(
            Id: 1,
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
