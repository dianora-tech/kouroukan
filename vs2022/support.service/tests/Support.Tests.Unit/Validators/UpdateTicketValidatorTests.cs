using FluentAssertions;
using FluentValidation.TestHelper;
using Support.Application.Commands;
using Support.Application.Validators;
using Xunit;

namespace Support.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateTicketValidator.
/// </summary>
public sealed class UpdateTicketValidatorTests
{
    private readonly UpdateTicketValidator _validator;

    public UpdateTicketValidatorTests()
    {
        _validator = new UpdateTicketValidator();
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

    // ─── StatutTicket ───

    [Theory]
    [InlineData("Ouvert")]
    [InlineData("EnCours")]
    [InlineData("EnAttente")]
    [InlineData("Resolu")]
    [InlineData("Ferme")]
    public void StatutTicket_ToutesLesValeursValides(string statut)
    {
        var command = CreateValidCommand() with { StatutTicket = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutTicket);
    }

    [Fact]
    public void StatutTicket_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutTicket = "Annule" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutTicket);
    }

    [Fact]
    public void StatutTicket_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutTicket = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutTicket);
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
    public void PieceJointeUrl_UrlInvalide_Erreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = "pas-une-url" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PieceJointeUrl);
    }

    [Fact]
    public void PieceJointeUrl_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PieceJointeUrl);
    }

    // ─── NoteSatisfaction ───

    [Fact]
    public void NoteSatisfaction_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { NoteSatisfaction = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NoteSatisfaction);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void NoteSatisfaction_EntreUnEtCinq_PasDerreur(int note)
    {
        var command = CreateValidCommand() with { NoteSatisfaction = note };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NoteSatisfaction);
    }

    [Fact]
    public void NoteSatisfaction_Zero_Erreur()
    {
        var command = CreateValidCommand() with { NoteSatisfaction = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NoteSatisfaction);
    }

    [Fact]
    public void NoteSatisfaction_SuperieurACinq_Erreur()
    {
        var command = CreateValidCommand() with { NoteSatisfaction = 6 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NoteSatisfaction);
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

    private static UpdateTicketCommand CreateValidCommand()
    {
        return new UpdateTicketCommand(
            Id: 1,
            Name: "Ticket de test",
            Description: "Description",
            TypeId: 1,
            AuteurId: 1,
            Sujet: "Probleme de connexion",
            Contenu: "Je ne peux pas me connecter",
            Priorite: "Moyenne",
            StatutTicket: "Ouvert",
            CategorieTicket: "Bug",
            ModuleConcerne: null,
            AssigneAId: null,
            DateResolution: null,
            NoteSatisfaction: null,
            PieceJointeUrl: null,
            UserId: 1);
    }
}
