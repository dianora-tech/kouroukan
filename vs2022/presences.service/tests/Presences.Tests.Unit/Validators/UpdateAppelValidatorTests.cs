using FluentAssertions;
using FluentValidation.TestHelper;
using Presences.Application.Commands;
using Presences.Application.Validators;
using Xunit;

namespace Presences.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateAppelValidator.
/// </summary>
public sealed class UpdateAppelValidatorTests
{
    private readonly UpdateAppelValidator _validator;

    public UpdateAppelValidatorTests()
    {
        _validator = new UpdateAppelValidator();
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

    // ─── DateAppel ───

    [Fact]
    public void DateAppel_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateAppel = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateAppel);
    }

    // ─── HeureAppel ───

    [Fact]
    public void HeureAppel_Zero_Erreur()
    {
        var command = CreateValidCommand() with { HeureAppel = TimeSpan.Zero };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.HeureAppel);
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

    private static UpdateAppelCommand CreateValidCommand()
    {
        return new UpdateAppelCommand(
            Id: 1,
            ClasseId: 5,
            EnseignantId: 3,
            SeanceId: null,
            DateAppel: new DateTime(2025, 9, 15),
            HeureAppel: new TimeSpan(8, 0, 0),
            EstCloture: false,
            UserId: 1);
    }
}
