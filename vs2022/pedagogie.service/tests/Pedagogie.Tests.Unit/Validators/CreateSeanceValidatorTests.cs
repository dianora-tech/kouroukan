using FluentAssertions;
using FluentValidation.TestHelper;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Validators;
using Xunit;

namespace Pedagogie.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateSeanceValidator.
/// </summary>
public sealed class CreateSeanceValidatorTests
{
    private readonly CreateSeanceValidator _validator;

    public CreateSeanceValidatorTests()
    {
        _validator = new CreateSeanceValidator();
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

    // ─── MatiereId ───

    [Fact]
    public void MatiereId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { MatiereId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MatiereId);
    }

    // ─── ClasseId ───

    [Fact]
    public void ClasseId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ClasseId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ClasseId);
    }

    // ─── EnseignantId ───

    [Fact]
    public void EnseignantId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EnseignantId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EnseignantId);
    }

    // ─── SalleId ───

    [Fact]
    public void SalleId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { SalleId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SalleId);
    }

    // ─── JourSemaine ───

    [Fact]
    public void JourSemaine_Zero_Erreur()
    {
        var command = CreateValidCommand() with { JourSemaine = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.JourSemaine);
    }

    [Fact]
    public void JourSemaine_Sept_Erreur()
    {
        var command = CreateValidCommand() with { JourSemaine = 7 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.JourSemaine);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(6)]
    public void JourSemaine_ValeurValide_PasDerreur(int jour)
    {
        var command = CreateValidCommand() with { JourSemaine = jour };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.JourSemaine);
    }

    // ─── HeureDebut / HeureFin ───

    [Fact]
    public void HeureFin_AvantDebut_Erreur()
    {
        var command = CreateValidCommand() with
        {
            HeureDebut = TimeSpan.FromHours(10),
            HeureFin = TimeSpan.FromHours(9)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.HeureFin);
    }

    [Fact]
    public void HeureFin_EgaleDebut_Erreur()
    {
        var command = CreateValidCommand() with
        {
            HeureDebut = TimeSpan.FromHours(10),
            HeureFin = TimeSpan.FromHours(10)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.HeureFin);
    }

    // ─── AnneeScolaireId ───

    [Fact]
    public void AnneeScolaireId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AnneeScolaireId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AnneeScolaireId);
    }

    // ─── Helper ───

    private static CreateSeanceCommand CreateValidCommand()
    {
        return new CreateSeanceCommand(
            Name: "Maths 7A Lundi 8h",
            Description: "Cours de maths",
            MatiereId: 1,
            ClasseId: 1,
            EnseignantId: 1,
            SalleId: 1,
            JourSemaine: 1,
            HeureDebut: TimeSpan.FromHours(8),
            HeureFin: TimeSpan.FromHours(10),
            AnneeScolaireId: 1);
    }
}
