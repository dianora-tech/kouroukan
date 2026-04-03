using FluentAssertions;
using FluentValidation.TestHelper;
using Presences.Application.Commands;
using Presences.Application.Validators;
using Xunit;

namespace Presences.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateBadgeageValidator.
/// </summary>
public sealed class CreateBadgeageValidatorTests
{
    private readonly CreateBadgeageValidator _validator;

    public CreateBadgeageValidatorTests()
    {
        _validator = new CreateBadgeageValidator();
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

    // ─── DateBadgeage ───

    [Fact]
    public void DateBadgeage_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateBadgeage = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateBadgeage);
    }

    // ─── PointAcces ───

    [Theory]
    [InlineData("Entree")]
    [InlineData("Sortie")]
    [InlineData("Cantine")]
    [InlineData("Biblio")]
    public void PointAcces_TousLesPointsValides(string pointAcces)
    {
        var command = CreateValidCommand() with { PointAcces = pointAcces };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PointAcces);
    }

    [Fact]
    public void PointAcces_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { PointAcces = "Inconnu" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PointAcces);
    }

    [Fact]
    public void PointAcces_Vide_Erreur()
    {
        var command = CreateValidCommand() with { PointAcces = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PointAcces);
    }

    // ─── MethodeBadgeage ───

    [Theory]
    [InlineData("NFC")]
    [InlineData("QRCode")]
    [InlineData("Manuel")]
    public void MethodeBadgeage_ToutesLesMethodesValides(string methode)
    {
        var command = CreateValidCommand() with { MethodeBadgeage = methode };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MethodeBadgeage);
    }

    [Fact]
    public void MethodeBadgeage_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { MethodeBadgeage = "Bluetooth" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MethodeBadgeage);
    }

    [Fact]
    public void MethodeBadgeage_Vide_Erreur()
    {
        var command = CreateValidCommand() with { MethodeBadgeage = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MethodeBadgeage);
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

    private static CreateBadgeageCommand CreateValidCommand()
    {
        return new CreateBadgeageCommand(
            TypeId: 1,
            EleveId: 10,
            DateBadgeage: new DateTime(2025, 9, 15),
            HeureBadgeage: new TimeSpan(7, 45, 0),
            PointAcces: "Entree",
            MethodeBadgeage: "NFC",
            UserId: 1);
    }
}
