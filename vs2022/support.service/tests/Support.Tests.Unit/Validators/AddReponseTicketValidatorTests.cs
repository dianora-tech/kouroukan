using FluentAssertions;
using FluentValidation.TestHelper;
using Support.Application.Commands;
using Support.Application.Validators;
using Xunit;

namespace Support.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour AddReponseTicketValidator.
/// </summary>
public sealed class AddReponseTicketValidatorTests
{
    private readonly AddReponseTicketValidator _validator;

    public AddReponseTicketValidatorTests()
    {
        _validator = new AddReponseTicketValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- TicketId ---

    [Fact]
    public void TicketId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { TicketId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TicketId);
    }

    [Fact]
    public void TicketId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { TicketId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TicketId);
    }

    // --- AuteurId ---

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

    // --- Contenu ---

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

    [Fact]
    public void Contenu_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Contenu = new string('A', 50000) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Contenu);
    }

    // --- UserId ---

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

    // --- Booleens ---

    [Fact]
    public void EstReponseIA_True_PasDerreur()
    {
        var command = CreateValidCommand() with { EstReponseIA = true };

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EstInterne_True_PasDerreur()
    {
        var command = CreateValidCommand() with { EstInterne = true };

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }

    // --- Helper ---

    private static AddReponseTicketCommand CreateValidCommand()
    {
        return new AddReponseTicketCommand(
            TicketId: 1,
            AuteurId: 1,
            Contenu: "Voici la reponse au ticket",
            EstReponseIA: false,
            EstInterne: false,
            UserId: 1);
    }
}
