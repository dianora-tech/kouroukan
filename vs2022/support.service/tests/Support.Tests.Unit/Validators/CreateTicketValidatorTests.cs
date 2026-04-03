using FluentAssertions;
using FluentValidation.TestHelper;
using Support.Application.Commands;
using Support.Application.Validators;
using Xunit;

namespace Support.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateTicketValidator.
/// </summary>
public sealed class CreateTicketValidatorTests
{
    private readonly CreateTicketValidator _validator;

    public CreateTicketValidatorTests()
    {
        _validator = new CreateTicketValidator();
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
        var command = CreateValidCommand() with { Contenu = new string('A', 50001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    // ─── Priorite ───

    [Theory]
    [InlineData("Basse")]
    [InlineData("Moyenne")]
    [InlineData("Haute")]
    [InlineData("Critique")]
    public void Priorite_ToutesLesValeursValides(string priorite)
    {
        var command = CreateValidCommand() with { Priorite = priorite };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Priorite);
    }

    [Fact]
    public void Priorite_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Priorite = "Urgente" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Priorite);
    }

    [Fact]
    public void Priorite_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Priorite = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Priorite);
    }

    // ─── CategorieTicket ───

    [Theory]
    [InlineData("Bug")]
    [InlineData("Question")]
    [InlineData("Amelioration")]
    [InlineData("Autre")]
    public void CategorieTicket_ToutesLesValeursValides(string categorie)
    {
        var command = CreateValidCommand() with { CategorieTicket = categorie };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CategorieTicket);
    }

    [Fact]
    public void CategorieTicket_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { CategorieTicket = "Incident" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CategorieTicket);
    }

    [Fact]
    public void CategorieTicket_Vide_Erreur()
    {
        var command = CreateValidCommand() with { CategorieTicket = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CategorieTicket);
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

    // ─── PieceJointeUrl ───

    [Fact]
    public void PieceJointeUrl_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PieceJointeUrl);
    }

    [Fact]
    public void PieceJointeUrl_UrlValide_PasDerreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = "https://example.com/file.pdf" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PieceJointeUrl);
    }

    [Fact]
    public void PieceJointeUrl_UrlInvalide_Erreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = "pas-une-url" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PieceJointeUrl);
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

    private static CreateTicketCommand CreateValidCommand()
    {
        return new CreateTicketCommand(
            Name: "Ticket de test",
            Description: "Description",
            TypeId: 1,
            AuteurId: 1,
            Sujet: "Probleme de connexion",
            Contenu: "Je ne peux pas me connecter a mon compte",
            Priorite: "Moyenne",
            CategorieTicket: "Bug",
            ModuleConcerne: "Auth",
            PieceJointeUrl: null,
            UserId: 1);
    }
}
