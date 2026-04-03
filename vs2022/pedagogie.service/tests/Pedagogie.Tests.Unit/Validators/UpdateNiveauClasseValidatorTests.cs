using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateNiveauClasseValidator.
/// </summary>
public sealed class UpdateNiveauClasseValidatorTests
{
    private readonly UpdateNiveauClasseValidator _validator;

    public UpdateNiveauClasseValidatorTests()
    {
        _validator = new UpdateNiveauClasseValidator();
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

    // ─── Code ───

    [Fact]
    public void Code_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Code = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    // ─── Ordre ───

    [Fact]
    public void Ordre_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Ordre = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Ordre);
    }

    // ─── CycleEtude ───

    [Fact]
    public void CycleEtude_Vide_Erreur()
    {
        var command = CreateValidCommand() with { CycleEtude = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CycleEtude);
    }

    [Fact]
    public void CycleEtude_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { CycleEtude = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CycleEtude);
    }

    // ─── MinistereTutelle ───

    [Fact]
    public void MinistereTutelle_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { MinistereTutelle = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MinistereTutelle);
    }

    // ─── Helper ───

    private static UpdateNiveauClasseCommand CreateValidCommand()
    {
        return new UpdateNiveauClasseCommand(
            Id: 1,
            Name: "7eme annee",
            Description: "Niveau 7eme",
            TypeId: 1,
            Code: "7E",
            Ordre: 7,
            CycleEtude: "College",
            AgeOfficielEntree: 13,
            MinistereTutelle: "MENA",
            ExamenSortie: null,
            TauxHoraireEnseignant: null);
    }
}
