using FluentAssertions;
using FluentValidation.TestHelper;
using Support.Application.Commands;
using Support.Application.Validators;
using Xunit;

namespace Support.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateSuggestionValidator.
/// </summary>
public sealed class UpdateSuggestionValidatorTests
{
    private readonly UpdateSuggestionValidator _validator;

    public UpdateSuggestionValidatorTests()
    {
        _validator = new UpdateSuggestionValidator();
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

    // ─── AuteurId ───

    [Fact]
    public void AuteurId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AuteurId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AuteurId);
    }

    // ─── Titre ───

    [Fact]
    public void Titre_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Titre = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Titre);
    }

    [Fact]
    public void Titre_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Titre = new string('A', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Titre);
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
    public void Contenu_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Contenu = new string('A', 50001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    // ─── ModuleConcerne ───

    [Fact]
    public void ModuleConcerne_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { ModuleConcerne = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ModuleConcerne);
    }

    [Fact]
    public void ModuleConcerne_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { ModuleConcerne = new string('A', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ModuleConcerne);
    }

    // ─── StatutSuggestion ───

    [Theory]
    [InlineData("Soumise")]
    [InlineData("EnRevue")]
    [InlineData("Acceptee")]
    [InlineData("Planifiee")]
    [InlineData("Realisee")]
    [InlineData("Rejetee")]
    public void StatutSuggestion_ToutesLesValeursValides(string statut)
    {
        var command = CreateValidCommand() with { StatutSuggestion = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutSuggestion);
    }

    [Fact]
    public void StatutSuggestion_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutSuggestion = "Annulee" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutSuggestion);
    }

    [Fact]
    public void StatutSuggestion_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutSuggestion = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutSuggestion);
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

    private static UpdateSuggestionCommand CreateValidCommand()
    {
        return new UpdateSuggestionCommand(
            Id: 1,
            Name: "Suggestion de test",
            Description: "Description",
            TypeId: 1,
            AuteurId: 1,
            Titre: "Ajouter un mode sombre",
            Contenu: "Il serait bien d'avoir un mode sombre",
            ModuleConcerne: "UI",
            StatutSuggestion: "Soumise",
            CommentaireAdmin: null,
            UserId: 1);
    }
}
