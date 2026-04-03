using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateNiveauClasseValidator.
/// </summary>
public sealed class CreateNiveauClasseValidatorTests
{
    private readonly CreateNiveauClasseValidator _validator;

    public CreateNiveauClasseValidatorTests()
    {
        _validator = new CreateNiveauClasseValidator();
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

    // ─── Ordre ───

    [Fact]
    public void Ordre_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Ordre = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Ordre);
    }

    [Fact]
    public void Ordre_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { Ordre = -1 };

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

    [Theory]
    [InlineData("Prescolaire")]
    [InlineData("Primaire")]
    [InlineData("College")]
    [InlineData("Lycee")]
    [InlineData("ETFP_PostPrimaire")]
    [InlineData("ETFP_TypeA")]
    [InlineData("ETFP_TypeB")]
    [InlineData("ENF")]
    [InlineData("Universite")]
    public void CycleEtude_TousLesCyclesValides(string cycle)
    {
        var command = CreateValidCommand() with { CycleEtude = cycle };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CycleEtude);
    }

    [Fact]
    public void CycleEtude_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { CycleEtude = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CycleEtude);
    }

    // ─── MinistereTutelle (optionnel) ───

    [Fact]
    public void MinistereTutelle_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { MinistereTutelle = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MinistereTutelle);
    }

    [Theory]
    [InlineData("MENA")]
    [InlineData("METFP-ET")]
    [InlineData("MESRS")]
    public void MinistereTutelle_Valide_PasDerreur(string ministere)
    {
        var command = CreateValidCommand() with { MinistereTutelle = ministere };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MinistereTutelle);
    }

    [Fact]
    public void MinistereTutelle_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { MinistereTutelle = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MinistereTutelle);
    }

    // ─── Helper ───

    private static CreateNiveauClasseCommand CreateValidCommand()
    {
        return new CreateNiveauClasseCommand(
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
