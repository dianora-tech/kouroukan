using FluentAssertions;
using FluentValidation.TestHelper;
using Support.Application.Commands;
using Support.Application.Validators;
using Xunit;

namespace Support.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateSuggestionValidator.
/// </summary>
public sealed class CreateSuggestionValidatorTests
{
    private readonly CreateSuggestionValidator _validator;

    public CreateSuggestionValidatorTests()
    {
        _validator = new CreateSuggestionValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ─── AuteurId ───

    [Fact]
    public void AuteurId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AuteurId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AuteurId);
    }

    [Fact]
    public void AuteurId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { AuteurId = -1 };

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

    [Fact]
    public void Titre_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Titre = new string('A', 200) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Titre);
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

    [Fact]
    public void ModuleConcerne_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { ModuleConcerne = new string('A', 50) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ModuleConcerne);
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

    private static CreateSuggestionCommand CreateValidCommand()
    {
        return new CreateSuggestionCommand(
            Name: "Suggestion de test",
            Description: "Description",
            TypeId: 1,
            AuteurId: 1,
            Titre: "Ajouter un mode sombre",
            Contenu: "Il serait bien d'avoir un mode sombre dans l'application",
            ModuleConcerne: "UI",
            UserId: 1);
    }
}
