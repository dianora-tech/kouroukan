using FluentAssertions;
using FluentValidation.TestHelper;
using Evaluations.Application.Commands;
using Evaluations.Application.Validators;
using Xunit;

namespace Evaluations.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateBulletinValidator.
/// </summary>
public sealed class UpdateBulletinValidatorTests
{
    private readonly UpdateBulletinValidator _validator;

    public UpdateBulletinValidatorTests()
    {
        _validator = new UpdateBulletinValidator();
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

    // ─── EleveId ───

    [Fact]
    public void EleveId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    // ─── ClasseId ───

    [Fact]
    public void ClasseId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ClasseId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ClasseId);
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

    // ─── MoyenneGenerale ───

    [Fact]
    public void MoyenneGenerale_Negative_Erreur()
    {
        var command = CreateValidCommand() with { MoyenneGenerale = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MoyenneGenerale);
    }

    [Fact]
    public void MoyenneGenerale_SuperieureA20_Erreur()
    {
        var command = CreateValidCommand() with { MoyenneGenerale = 21 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MoyenneGenerale);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(20)]
    public void MoyenneGenerale_Valide_PasDerreur(int moyenne)
    {
        var command = CreateValidCommand() with { MoyenneGenerale = moyenne };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MoyenneGenerale);
    }

    // ─── Appreciation ───

    [Fact]
    public void Appreciation_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Appreciation = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Appreciation);
    }

    [Fact]
    public void Appreciation_TropLongue_Erreur()
    {
        var command = CreateValidCommand() with { Appreciation = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Appreciation);
    }

    // ─── DateGeneration ───

    [Fact]
    public void DateGeneration_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateGeneration = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateGeneration);
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

    private static UpdateBulletinCommand CreateValidCommand()
    {
        return new UpdateBulletinCommand(
            Id: 1,
            EleveId: 5,
            ClasseId: 3,
            Trimestre: 1,
            AnneeScolaireId: 1,
            MoyenneGenerale: 14.5m,
            Rang: 3,
            Appreciation: "Bon trimestre",
            EstPublie: false,
            DateGeneration: new DateTime(2025, 12, 20),
            CheminFichierPdf: null,
            UserId: 1);
    }
}
