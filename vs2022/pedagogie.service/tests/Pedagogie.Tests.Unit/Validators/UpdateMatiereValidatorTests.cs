using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateMatiereValidator.
/// </summary>
public sealed class UpdateMatiereValidatorTests
{
    private readonly UpdateMatiereValidator _validator;

    public UpdateMatiereValidatorTests()
    {
        _validator = new UpdateMatiereValidator();
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

    // ─── Code ───

    [Fact]
    public void Code_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Code = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    // ─── Helper ───

    private static UpdateMatiereCommand CreateValidCommand()
    {
        return new UpdateMatiereCommand(
            Id: 1,
            Name: "Mathematiques",
            Description: "Cours de maths",
            TypeId: 1,
            Code: "MATH");
    }
}
