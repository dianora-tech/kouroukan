using FluentAssertions;
using FluentValidation.TestHelper;
using Communication.Application.Commands;
using Communication.Application.Validators;
using Xunit;

namespace Communication.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateAnnonceValidator.
/// </summary>
public sealed class CreateAnnonceValidatorTests
{
    private readonly CreateAnnonceValidator _validator;

    public CreateAnnonceValidatorTests()
    {
        _validator = new CreateAnnonceValidator();
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

    // ─── Contenu ───

    [Fact]
    public void Contenu_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Contenu = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    [Fact]
    public void Contenu_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Contenu = new string('A', 10001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Contenu);
    }

    // ─── DateDebut ───

    [Fact]
    public void DateDebut_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateDebut = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateDebut);
    }

    // ─── CibleAudience ───

    [Theory]
    [InlineData("Tous")]
    [InlineData("Parents")]
    [InlineData("Enseignants")]
    [InlineData("Eleves")]
    public void CibleAudience_ToutesLesValeursValides(string cible)
    {
        var command = CreateValidCommand() with { CibleAudience = cible };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CibleAudience);
    }

    [Fact]
    public void CibleAudience_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { CibleAudience = "Directeurs" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CibleAudience);
    }

    [Fact]
    public void CibleAudience_Vide_Erreur()
    {
        var command = CreateValidCommand() with { CibleAudience = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CibleAudience);
    }

    // ─── Priorite ───

    [Fact]
    public void Priorite_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Priorite = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Priorite);
    }

    [Fact]
    public void Priorite_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { Priorite = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Priorite);
    }

    [Fact]
    public void Priorite_Un_PasDerreur()
    {
        var command = CreateValidCommand() with { Priorite = 1 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Priorite);
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

    private static CreateAnnonceCommand CreateValidCommand()
    {
        return new CreateAnnonceCommand(
            Name: "Annonce de test",
            TypeId: 1,
            Contenu: "Contenu de l'annonce",
            DateDebut: new DateTime(2025, 9, 1),
            DateFin: new DateTime(2025, 12, 31),
            EstActive: true,
            CibleAudience: "Tous",
            Priorite: 1,
            UserId: 1);
    }
}
