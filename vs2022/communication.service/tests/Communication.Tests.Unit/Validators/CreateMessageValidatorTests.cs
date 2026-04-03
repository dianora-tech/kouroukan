using FluentAssertions;
using FluentValidation.TestHelper;
using Communication.Application.Commands;
using Communication.Application.Validators;
using Xunit;

namespace Communication.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateMessageValidator.
/// </summary>
public sealed class CreateMessageValidatorTests
{
    private readonly CreateMessageValidator _validator;

    public CreateMessageValidatorTests()
    {
        _validator = new CreateMessageValidator();
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

    // ─── ExpediteurId ───

    [Fact]
    public void ExpediteurId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ExpediteurId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpediteurId);
    }

    [Fact]
    public void ExpediteurId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { ExpediteurId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpediteurId);
    }

    // ─── Sujet ───

    [Fact]
    public void Sujet_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Sujet = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Sujet);
    }

    [Fact]
    public void Sujet_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Sujet = new string('A', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Sujet);
    }

    [Fact]
    public void Sujet_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Sujet = new string('A', 200) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Sujet);
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
        var command = CreateValidCommand() with { Contenu = new string('A', 10001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    // ─── GroupeDestinataire (optionnel) ───

    [Fact]
    public void GroupeDestinataire_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { GroupeDestinataire = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.GroupeDestinataire);
    }

    [Fact]
    public void GroupeDestinataire_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { GroupeDestinataire = new string('A', 101) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GroupeDestinataire);
    }

    [Fact]
    public void GroupeDestinataire_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { GroupeDestinataire = new string('A', 100) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.GroupeDestinataire);
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

    private static CreateMessageCommand CreateValidCommand()
    {
        return new CreateMessageCommand(
            TypeId: 1,
            ExpediteurId: 10,
            DestinataireId: 5,
            Sujet: "Sujet du message",
            Contenu: "Contenu du message",
            EstLu: false,
            DateLecture: null,
            GroupeDestinataire: null,
            UserId: 1);
    }
}
