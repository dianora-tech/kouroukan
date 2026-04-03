using FluentAssertions;
using FluentValidation.TestHelper;
using Communication.Application.Commands;
using Communication.Application.Validators;
using Xunit;

namespace Communication.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateNotificationValidator.
/// </summary>
public sealed class CreateNotificationValidatorTests
{
    private readonly CreateNotificationValidator _validator;

    public CreateNotificationValidatorTests()
    {
        _validator = new CreateNotificationValidator();
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

    // ─── DestinatairesIds ───

    [Fact]
    public void DestinatairesIds_Vide_Erreur()
    {
        var command = CreateValidCommand() with { DestinatairesIds = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DestinatairesIds);
    }

    [Fact]
    public void DestinatairesIds_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { DestinatairesIds = new string('1', 10001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DestinatairesIds);
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
        var command = CreateValidCommand() with { Contenu = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    [Fact]
    public void Contenu_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Contenu = new string('A', 500) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Contenu);
    }

    // ─── Canal ───

    [Theory]
    [InlineData("Push")]
    [InlineData("SMS")]
    [InlineData("Email")]
    [InlineData("InApp")]
    public void Canal_ToutesLesValeursValides(string canal)
    {
        var command = CreateValidCommand() with { Canal = canal };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Canal);
    }

    [Fact]
    public void Canal_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Canal = "Fax" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Canal);
    }

    [Fact]
    public void Canal_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Canal = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Canal);
    }

    // ─── LienAction (optionnel URL) ───

    [Fact]
    public void LienAction_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { LienAction = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.LienAction);
    }

    [Fact]
    public void LienAction_UrlValide_PasDerreur()
    {
        var command = CreateValidCommand() with { LienAction = "https://example.com/action" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.LienAction);
    }

    [Fact]
    public void LienAction_UrlInvalide_Erreur()
    {
        var command = CreateValidCommand() with { LienAction = "pas-une-url" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LienAction);
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

    private static CreateNotificationCommand CreateValidCommand()
    {
        return new CreateNotificationCommand(
            TypeId: 1,
            DestinatairesIds: "[1, 2, 3]",
            Contenu: "Contenu de la notification",
            Canal: "Push",
            EstEnvoyee: false,
            DateEnvoi: null,
            LienAction: null,
            UserId: 1);
    }
}
