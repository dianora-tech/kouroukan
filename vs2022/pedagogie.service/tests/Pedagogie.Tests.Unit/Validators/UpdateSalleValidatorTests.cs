using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateSalleValidator.
/// </summary>
public sealed class UpdateSalleValidatorTests
{
    private readonly UpdateSalleValidator _validator;

    public UpdateSalleValidatorTests()
    {
        _validator = new UpdateSalleValidator();
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

    // ─── Name ───

    [Fact]
    public void Name_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Name = "" };

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

    // ─── Helper ───

    private static UpdateSalleCommand CreateValidCommand()
    {
        return new UpdateSalleCommand(
            Id: 1,
            Name: "Salle 101",
            Description: "Salle principale",
            TypeId: 1,
            Capacite: 40,
            Batiment: "Batiment A",
            Equipements: "Tableau",
            EstDisponible: true);
    }
}
