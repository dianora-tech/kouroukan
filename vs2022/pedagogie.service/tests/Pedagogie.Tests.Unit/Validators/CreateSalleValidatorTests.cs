using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateSalleValidator.
/// </summary>
public sealed class CreateSalleValidatorTests
{
    private readonly CreateSalleValidator _validator;

    public CreateSalleValidatorTests()
    {
        _validator = new CreateSalleValidator();
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
        var command = CreateValidCommand() with { Name = new string('A', 101) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // ─── Capacite ───

    [Fact]
    public void Capacite_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Capacite = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Capacite);
    }

    [Fact]
    public void Capacite_Negative_Erreur()
    {
        var command = CreateValidCommand() with { Capacite = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Capacite);
    }

    // ─── Helper ───

    private static CreateSalleCommand CreateValidCommand()
    {
        return new CreateSalleCommand(
            Name: "Salle 101",
            Description: "Salle principale",
            TypeId: 1,
            Capacite: 40,
            Batiment: "Batiment A",
            Equipements: "Tableau",
            EstDisponible: true);
    }
}
