using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateMatiereValidator.
/// </summary>
public sealed class CreateMatiereValidatorTests
{
    private readonly CreateMatiereValidator _validator;

    public CreateMatiereValidatorTests()
    {
        _validator = new CreateMatiereValidator();
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

    // ─── Code ───

    [Fact]
    public void Code_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Code = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Code_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Code = new string('A', 21) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    // ─── Helper ───

    private static CreateMatiereCommand CreateValidCommand()
    {
        return new CreateMatiereCommand(
            Name: "Mathematiques",
            Description: "Cours de maths",
            TypeId: 1,
            Code: "MATH");
    }
}
