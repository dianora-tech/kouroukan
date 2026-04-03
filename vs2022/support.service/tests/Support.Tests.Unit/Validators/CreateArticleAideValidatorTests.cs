using FluentAssertions;
using FluentValidation.TestHelper;
using Support.Application.Commands;
using Support.Application.Validators;
using Xunit;

namespace Support.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateArticleAideValidator.
/// </summary>
public sealed class CreateArticleAideValidatorTests
{
    private readonly CreateArticleAideValidator _validator;

    public CreateArticleAideValidatorTests()
    {
        _validator = new CreateArticleAideValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
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

    // ─── Slug ───

    [Fact]
    public void Slug_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Slug = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Fact]
    public void Slug_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Slug = new string('a', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Fact]
    public void Slug_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Slug = new string('a', 200) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Slug);
    }

    // ─── Categorie ───

    [Fact]
    public void Categorie_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Categorie = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Categorie);
    }

    [Fact]
    public void Categorie_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Categorie = new string('A', 101) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Categorie);
    }

    [Fact]
    public void Categorie_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Categorie = new string('A', 100) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Categorie);
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

    // ─── Ordre ───

    [Fact]
    public void Ordre_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { Ordre = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Ordre);
    }

    [Fact]
    public void Ordre_Positif_PasDerreur()
    {
        var command = CreateValidCommand() with { Ordre = 10 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Ordre);
    }

    [Fact]
    public void Ordre_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { Ordre = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Ordre);
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

    private static CreateArticleAideCommand CreateValidCommand()
    {
        return new CreateArticleAideCommand(
            Name: "Article de test",
            Description: "Description",
            TypeId: 1,
            Titre: "Comment se connecter",
            Contenu: "Pour vous connecter, suivez ces etapes...",
            Slug: "comment-se-connecter",
            Categorie: "Guide",
            ModuleConcerne: "Auth",
            Ordre: 1,
            EstPublie: true,
            UserId: 1);
    }
}
