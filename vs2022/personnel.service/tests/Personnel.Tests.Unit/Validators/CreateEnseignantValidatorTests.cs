using FluentAssertions;
using FluentValidation.TestHelper;
using Personnel.Application.Commands;
using Personnel.Application.Validators;
using Xunit;

namespace Personnel.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateEnseignantValidator.
/// </summary>
public sealed class CreateEnseignantValidatorTests
{
    private readonly CreateEnseignantValidator _validator;

    public CreateEnseignantValidatorTests()
    {
        _validator = new CreateEnseignantValidator();
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
        var command = CreateValidCommand() with { Name = new string('A', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { Name = new string('A', 200) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    // ─── Description ───

    [Fact]
    public void Description_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Description = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_TropLongue_Erreur()
    {
        var command = CreateValidCommand() with { Description = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    // ─── Matricule ───

    [Fact]
    public void Matricule_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Matricule = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Matricule);
    }

    [Fact]
    public void Matricule_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Matricule = new string('M', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Matricule);
    }

    // ─── Specialite ───

    [Fact]
    public void Specialite_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Specialite = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Specialite);
    }

    [Fact]
    public void Specialite_TropLongue_Erreur()
    {
        var command = CreateValidCommand() with { Specialite = new string('S', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Specialite);
    }

    // ─── DateEmbauche ───

    [Fact]
    public void DateEmbauche_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateEmbauche = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateEmbauche);
    }

    // ─── ModeRemuneration ───

    [Theory]
    [InlineData("Forfait")]
    [InlineData("Heures")]
    [InlineData("Mixte")]
    public void ModeRemuneration_Valide_PasDerreur(string mode)
    {
        var command = CreateValidCommand() with { ModeRemuneration = mode };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ModeRemuneration);
    }

    [Fact]
    public void ModeRemuneration_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { ModeRemuneration = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ModeRemuneration);
    }

    [Fact]
    public void ModeRemuneration_Vide_Erreur()
    {
        var command = CreateValidCommand() with { ModeRemuneration = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ModeRemuneration);
    }

    // ─── MontantForfait ───

    [Fact]
    public void MontantForfait_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { MontantForfait = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MontantForfait);
    }

    [Fact]
    public void MontantForfait_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { MontantForfait = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MontantForfait);
    }

    [Fact]
    public void MontantForfait_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { MontantForfait = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MontantForfait);
    }

    // ─── Telephone ───

    [Fact]
    public void Telephone_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Telephone = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Telephone);
    }

    [Fact]
    public void Telephone_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Telephone = new string('0', 21) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Telephone);
    }

    // ─── Email ───

    [Fact]
    public void Email_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { Email = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Email_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Email = new string('e', 201) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    // ─── StatutEnseignant ───

    [Theory]
    [InlineData("Actif")]
    [InlineData("EnConge")]
    [InlineData("Suspendu")]
    [InlineData("Inactif")]
    public void StatutEnseignant_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutEnseignant = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutEnseignant);
    }

    [Fact]
    public void StatutEnseignant_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutEnseignant = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutEnseignant);
    }

    [Fact]
    public void StatutEnseignant_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutEnseignant = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutEnseignant);
    }

    // ─── SoldeCongesAnnuel ───

    [Fact]
    public void SoldeCongesAnnuel_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { SoldeCongesAnnuel = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.SoldeCongesAnnuel);
    }

    [Fact]
    public void SoldeCongesAnnuel_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { SoldeCongesAnnuel = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SoldeCongesAnnuel);
    }

    // ─── TypeId ───

    [Fact]
    public void TypeId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypeId);
    }

    [Fact]
    public void TypeId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypeId);
    }

    // ─── UserId ───

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void UserId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { UserId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ─── Helper ───

    private static CreateEnseignantCommand CreateValidCommand()
    {
        return new CreateEnseignantCommand(
            Name: "Mamadou Diallo",
            Description: "Professeur de mathematiques",
            Matricule: "MAT-001",
            Specialite: "Mathematiques",
            DateEmbauche: new DateTime(2020, 9, 1),
            ModeRemuneration: "Forfait",
            MontantForfait: 500000m,
            Telephone: "+224 620 00 00 00",
            Email: "mamadou.diallo@test.com",
            StatutEnseignant: "Actif",
            SoldeCongesAnnuel: 30,
            TypeId: 1,
            UserId: 1);
    }
}
