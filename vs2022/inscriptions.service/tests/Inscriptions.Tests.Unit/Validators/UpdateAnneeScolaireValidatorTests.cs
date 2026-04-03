using FluentAssertions;
using FluentValidation.TestHelper;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Validators;
using Xunit;

namespace Inscriptions.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateAnneeScolaireValidator.
/// </summary>
public sealed class UpdateAnneeScolaireValidatorTests
{
    private readonly UpdateAnneeScolaireValidator _validator;

    public UpdateAnneeScolaireValidatorTests()
    {
        _validator = new UpdateAnneeScolaireValidator();
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

    // ─── Libelle ───

    [Fact]
    public void Libelle_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Libelle = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Libelle);
    }

    [Fact]
    public void Libelle_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Libelle = new string('A', 21) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Libelle);
    }

    // ─── DateDebut ───

    [Fact]
    public void DateDebut_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateDebut = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateDebut);
    }

    // ─── DateFin ───

    [Fact]
    public void DateFin_AnterieureDateDebut_Erreur()
    {
        var command = CreateValidCommand() with
        {
            DateDebut = new DateTime(2025, 10, 1),
            DateFin = new DateTime(2025, 9, 1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateFin);
    }

    // ─── Statut ───

    [Theory]
    [InlineData("preparation")]
    [InlineData("active")]
    [InlineData("cloturee")]
    [InlineData("archivee")]
    public void Statut_TousLesStatutsValides(string statut)
    {
        var command = CreateValidCommand() with { Statut = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Statut);
    }

    [Fact]
    public void Statut_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Statut = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Statut);
    }

    // ─── NombrePeriodes ───

    [Fact]
    public void NombrePeriodes_Zero_Erreur()
    {
        var command = CreateValidCommand() with { NombrePeriodes = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NombrePeriodes);
    }

    // ─── TypePeriode ───

    [Theory]
    [InlineData("trimestre")]
    [InlineData("semestre")]
    public void TypePeriode_Valide_PasDerreur(string type)
    {
        var command = CreateValidCommand() with { TypePeriode = type };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TypePeriode);
    }

    [Fact]
    public void TypePeriode_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { TypePeriode = "bimestre" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypePeriode);
    }

    // ─── Code (optionnel) ───

    [Fact]
    public void Code_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Code = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Code_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Code = new string('A', 21) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    // ─── Description (optionnel) ───

    [Fact]
    public void Description_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Description = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    // ─── Helper ───

    private static UpdateAnneeScolaireCommand CreateValidCommand()
    {
        return new UpdateAnneeScolaireCommand(
            Id: 1,
            Libelle: "2025-2026",
            DateDebut: new DateTime(2025, 10, 1),
            DateFin: new DateTime(2026, 6, 30),
            EstActive: true,
            Code: "2025-2026",
            Description: null,
            Statut: "preparation",
            DateRentree: null,
            NombrePeriodes: 3,
            TypePeriode: "trimestre");
    }
}
