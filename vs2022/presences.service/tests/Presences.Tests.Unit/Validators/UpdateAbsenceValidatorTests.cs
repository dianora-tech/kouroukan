using FluentAssertions;
using FluentValidation.TestHelper;
using Presences.Application.Commands;
using Presences.Application.Validators;
using Xunit;

namespace Presences.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateAbsenceValidator.
/// </summary>
public sealed class UpdateAbsenceValidatorTests
{
    private readonly UpdateAbsenceValidator _validator;

    public UpdateAbsenceValidatorTests()
    {
        _validator = new UpdateAbsenceValidator();
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

    // ─── EleveId ───

    [Fact]
    public void EleveId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = 0 };

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

    // ─── MotifJustification ───

    [Fact]
    public void MotifJustification_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { MotifJustification = new string('a', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MotifJustification);
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
    public void PieceJointeUrl_UrlValide_PasDerreur()
    {
        var command = CreateValidCommand() with { PieceJointeUrl = "https://example.com/cert.pdf" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PieceJointeUrl);
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

    private static UpdateAbsenceCommand CreateValidCommand()
    {
        return new UpdateAbsenceCommand(
            Id: 1,
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
