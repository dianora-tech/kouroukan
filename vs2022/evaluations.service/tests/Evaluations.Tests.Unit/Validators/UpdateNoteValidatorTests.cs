using FluentAssertions;
using FluentValidation.TestHelper;
using Evaluations.Application.Commands;
using Evaluations.Application.Validators;
using Xunit;

namespace Evaluations.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateNoteValidator.
/// </summary>
public sealed class UpdateNoteValidatorTests
{
    private readonly UpdateNoteValidator _validator;

    public UpdateNoteValidatorTests()
    {
        _validator = new UpdateNoteValidator();
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

    // ─── EvaluationId ───

    [Fact]
    public void EvaluationId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EvaluationId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EvaluationId);
    }

    // ─── EleveId ───

    [Fact]
    public void EleveId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    // ─── Valeur ───

    [Fact]
    public void Valeur_Negative_Erreur()
    {
        var command = CreateValidCommand() with { Valeur = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Valeur);
    }

    [Fact]
    public void Valeur_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { Valeur = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Valeur);
    }

    // ─── Commentaire ───

    [Fact]
    public void Commentaire_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Commentaire = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Commentaire);
    }

    [Fact]
    public void Commentaire_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Commentaire = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Commentaire);
    }

    // ─── DateSaisie ───

    [Fact]
    public void DateSaisie_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateSaisie = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateSaisie);
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

    private static UpdateNoteCommand CreateValidCommand()
    {
        return new UpdateNoteCommand(
            Id: 1,
            EvaluationId: 10,
            EleveId: 5,
            Valeur: 15.5m,
            Commentaire: "Bon travail",
            DateSaisie: new DateTime(2025, 10, 20),
            UserId: 1);
    }
}
