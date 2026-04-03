using FluentAssertions;
using FluentValidation.TestHelper;
using Evaluations.Application.Commands;
using Evaluations.Application.Validators;
using Xunit;

namespace Evaluations.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateEvaluationValidator.
/// </summary>
public sealed class UpdateEvaluationValidatorTests
{
    private readonly UpdateEvaluationValidator _validator;

    public UpdateEvaluationValidatorTests()
    {
        _validator = new UpdateEvaluationValidator();
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

    // ─── MatiereId ───

    [Fact]
    public void MatiereId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { MatiereId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MatiereId);
    }

    // ─── ClasseId ───

    [Fact]
    public void ClasseId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ClasseId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ClasseId);
    }

    // ─── EnseignantId ───

    [Fact]
    public void EnseignantId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EnseignantId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EnseignantId);
    }

    // ─── DateEvaluation ───

    [Fact]
    public void DateEvaluation_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateEvaluation = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateEvaluation);
    }

    // ─── Coefficient ───

    [Fact]
    public void Coefficient_Zero_Valide()
    {
        var command = CreateValidCommand() with { Coefficient = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Coefficient);
    }

    // ─── NoteMaximale ───

    [Fact]
    public void NoteMaximale_Zero_Valide()
    {
        var command = CreateValidCommand() with { NoteMaximale = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NoteMaximale);
    }

    // ─── Trimestre ───

    [Fact]
    public void Trimestre_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Trimestre = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Trimestre);
    }

    [Fact]
    public void Trimestre_Quatre_Erreur()
    {
        var command = CreateValidCommand() with { Trimestre = 4 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Trimestre);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Trimestre_Valide_PasDerreur(int trimestre)
    {
        var command = CreateValidCommand() with { Trimestre = trimestre };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Trimestre);
    }

    // ─── AnneeScolaireId ───

    [Fact]
    public void AnneeScolaireId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AnneeScolaireId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AnneeScolaireId);
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

    private static UpdateEvaluationCommand CreateValidCommand()
    {
        return new UpdateEvaluationCommand(
            Id: 1,
            TypeId: 1,
            MatiereId: 10,
            ClasseId: 5,
            EnseignantId: 3,
            DateEvaluation: new DateTime(2025, 10, 15),
            Coefficient: 2m,
            NoteMaximale: 20m,
            Trimestre: 1,
            AnneeScolaireId: 1,
            UserId: 1);
    }
}
