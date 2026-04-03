using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateClasseValidator.
/// </summary>
public sealed class UpdateClasseValidatorTests
{
    private readonly UpdateClasseValidator _validator;

    public UpdateClasseValidatorTests()
    {
        _validator = new UpdateClasseValidator();
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

    // ─── NiveauClasseId ───

    [Fact]
    public void NiveauClasseId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { NiveauClasseId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NiveauClasseId);
    }

    // ─── AnneeScolaireId ───

    [Fact]
    public void AnneeScolaireId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AnneeScolaireId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AnneeScolaireId);
    }

    // ─── Effectif ───

    [Fact]
    public void Effectif_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { Effectif = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Effectif);
    }

    // ─── Helper ───

    private static UpdateClasseCommand CreateValidCommand()
    {
        return new UpdateClasseCommand(
            Id: 1,
            Name: "7eme A",
            Description: "Classe de 7eme",
            NiveauClasseId: 1,
            Capacite: 40,
            AnneeScolaireId: 1,
            EnseignantPrincipalId: null,
            Effectif: 30);
    }
}
