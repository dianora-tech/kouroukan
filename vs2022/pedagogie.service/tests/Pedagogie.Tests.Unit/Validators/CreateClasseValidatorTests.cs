using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateClasseValidator.
/// </summary>
public sealed class CreateClasseValidatorTests
{
    private readonly CreateClasseValidator _validator;

    public CreateClasseValidatorTests()
    {
        _validator = new CreateClasseValidator();
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

    [Fact]
    public void Effectif_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { Effectif = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Effectif);
    }

    // ─── Helper ───

    private static CreateClasseCommand CreateValidCommand()
    {
        return new CreateClasseCommand(
            Name: "7eme A",
            Description: "Classe de 7eme",
            NiveauClasseId: 1,
            Capacite: 40,
            AnneeScolaireId: 1,
            EnseignantPrincipalId: null,
            Effectif: 30);
    }
}
