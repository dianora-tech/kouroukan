using FluentAssertions;
using FluentValidation.TestHelper;
using Presences.Application.Commands;
using Presences.Application.Validators;
using Xunit;

namespace Presences.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateAbsenceValidator.
/// </summary>
public sealed class CreateAbsenceValidatorTests
{
    private readonly CreateAbsenceValidator _validator;

    public CreateAbsenceValidatorTests()
    {
        _validator = new CreateAbsenceValidator();
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

    // ─── EleveId ───

    [Fact]
    public void EleveId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    [Fact]
    public void EleveId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    // ─── DateAbsence ───

    [Fact]
    public void DateAbsence_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateAbsence = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateAbsence);
    }

    // ─── MotifJustification (optionnel, max 500) ───

    [Fact]
    public void MotifJustification_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { MotifJustification = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MotifJustification);
    }

    [Fact]
    public void MotifJustification_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { MotifJustification = new string('a', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MotifJustification);
    }

    [Fact]
    public void MotifJustification_500Caracteres_PasDerreur()
    {
        var command = CreateValidCommand() with { MotifJustification = new string('a', 500) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MotifJustification);
    }

    // ─── PieceJointeUrl (optionnel, URL) ───

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
        var command = CreateValidCommand() with { PieceJointeUrl = "https://example.com/doc.pdf" };

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

    private static CreateAbsenceCommand CreateValidCommand()
    {
        return new CreateAbsenceCommand(
            TypeId: 1,
            EleveId: 10,
            AppelId: null,
            DateAbsence: new DateTime(2025, 9, 15),
            HeureDebut: new TimeSpan(8, 0, 0),
            HeureFin: new TimeSpan(10, 0, 0),
            EstJustifiee: false,
            MotifJustification: null,
            PieceJointeUrl: null,
            UserId: 1);
    }
}
