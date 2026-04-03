using FluentAssertions;
using FluentValidation.TestHelper;
using Bde.Application.Commands;
using Bde.Application.Validators;
using Xunit;

namespace Bde.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateMembreBdeValidator.
/// </summary>
public sealed class CreateMembreBdeValidatorTests
{
    private readonly CreateMembreBdeValidator _validator;

    public CreateMembreBdeValidatorTests()
    {
        _validator = new CreateMembreBdeValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
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

    // ─── Description (optionnel) ───

    [Fact]
    public void Description_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Description = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_TropLongue_Erreur()
    {
        var command = CreateValidCommand() with { Description = new string('A', 1001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    // ─── AssociationId ───

    [Fact]
    public void AssociationId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AssociationId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AssociationId);
    }

    // ─── EleveId ───

    [Fact]
    public void EleveId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    // ─── RoleBde ───

    [Theory]
    [InlineData("President")]
    [InlineData("Tresorier")]
    [InlineData("Secretaire")]
    [InlineData("RespPole")]
    [InlineData("Membre")]
    public void RoleBde_Valide_PasDerreur(string role)
    {
        var command = CreateValidCommand() with { RoleBde = role };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RoleBde);
    }

    [Fact]
    public void RoleBde_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { RoleBde = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RoleBde);
    }

    [Fact]
    public void RoleBde_Vide_Erreur()
    {
        var command = CreateValidCommand() with { RoleBde = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RoleBde);
    }

    // ─── DateAdhesion ───

    [Fact]
    public void DateAdhesion_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateAdhesion = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateAdhesion);
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

    private static CreateMembreBdeCommand CreateValidCommand()
    {
        return new CreateMembreBdeCommand(
            Name: "Jean Dupont",
            Description: "Membre actif",
            AssociationId: 5,
            EleveId: 10,
            RoleBde: "Membre",
            DateAdhesion: new DateTime(2025, 9, 1),
            MontantCotisation: 25000m,
            EstActif: true,
            UserId: 1);
    }
}
