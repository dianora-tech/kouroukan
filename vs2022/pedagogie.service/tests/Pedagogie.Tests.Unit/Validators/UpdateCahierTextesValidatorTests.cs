using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateCahierTextesValidator.
/// </summary>
public sealed class UpdateCahierTextesValidatorTests
{
    private readonly UpdateCahierTextesValidator _validator;

    public UpdateCahierTextesValidatorTests()
    {
        _validator = new UpdateCahierTextesValidator();
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

    // ─── Name ───

    [Fact]
    public void Name_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Name = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Name = new string('A', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // ─── SeanceId ───

    [Fact]
    public void SeanceId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { SeanceId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SeanceId);
    }

    // ─── Contenu ───

    [Fact]
    public void Contenu_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Contenu = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    // ─── DateSeance ───

    [Fact]
    public void DateSeance_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateSeance = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateSeance);
    }

    // ─── Helper ───

    private static UpdateCahierTextesCommand CreateValidCommand()
    {
        return new UpdateCahierTextesCommand(
            Id: 1,
            Name: "Cours du 01/09",
            Description: "Cahier de textes",
            SeanceId: 1,
            Contenu: "Introduction aux equations",
            DateSeance: new DateTime(2025, 9, 1),
            TravailAFaire: "Exercices page 12");
    }
}
