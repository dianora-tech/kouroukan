using FluentAssertions;
using FluentValidation.TestHelper;
using Communication.Application.Commands;
using Communication.Application.Validators;
using Xunit;

namespace Communication.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateMessageValidator.
/// </summary>
public sealed class UpdateMessageValidatorTests
{
    private readonly UpdateMessageValidator _validator;

    public UpdateMessageValidatorTests()
    {
        _validator = new UpdateMessageValidator();
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

    // ─── ExpediteurId ───

    [Fact]
    public void ExpediteurId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ExpediteurId = 0 };

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
    public void GroupeDestinataire_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { GroupeDestinataire = new string('A', 101) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GroupeDestinataire);
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

    private static UpdateMessageCommand CreateValidCommand()
    {
        return new UpdateMessageCommand(
            Id: 1,
            TypeId: 1,
            ExpediteurId: 10,
            DestinataireId: 5,
            Sujet: "Sujet modifie",
            Contenu: "Contenu modifie",
            EstLu: true,
            DateLecture: DateTime.Now,
            GroupeDestinataire: null,
            UserId: 1);
    }
}
