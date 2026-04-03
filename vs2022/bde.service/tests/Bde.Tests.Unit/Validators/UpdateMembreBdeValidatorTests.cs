using FluentAssertions;
using FluentValidation.TestHelper;
using Bde.Application.Commands;
using Bde.Application.Validators;
using Xunit;

namespace Bde.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateMembreBdeValidator.
/// </summary>
public sealed class UpdateMembreBdeValidatorTests
{
    private readonly UpdateMembreBdeValidator _validator;

    public UpdateMembreBdeValidatorTests()
    {
        _validator = new UpdateMembreBdeValidator();
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

    [Fact]
    public void RoleBde_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { RoleBde = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RoleBde);
    }

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

    private static UpdateMembreBdeCommand CreateValidCommand()
    {
        return new UpdateMembreBdeCommand(
            Id: 1,
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
