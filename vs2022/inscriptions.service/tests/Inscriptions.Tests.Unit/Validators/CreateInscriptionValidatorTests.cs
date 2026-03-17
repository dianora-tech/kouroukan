using FluentAssertions;
using FluentValidation.TestHelper;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Validators;
using Xunit;

namespace Inscriptions.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateInscriptionValidator.
/// </summary>
public sealed class CreateInscriptionValidatorTests
{
    private readonly CreateInscriptionValidator _validator;

    public CreateInscriptionValidatorTests()
    {
        _validator = new CreateInscriptionValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
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

    // ─── EleveId ───

    [Fact]
    public void EleveId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    // ─── ClasseId ───

    [Fact]
    public void ClasseId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ClasseId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ClasseId);
    }

    // ─── AnneeScolaireId ───

    [Fact]
    public void AnneeScolaireId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AnneeScolaireId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AnneeScolaireId);
    }

    // ─── DateInscription ───

    [Fact]
    public void DateInscription_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateInscription = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateInscription);
    }

    // ─── MontantInscription ───

    [Fact]
    public void MontantInscription_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { MontantInscription = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MontantInscription);
    }

    [Fact]
    public void MontantInscription_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { MontantInscription = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MontantInscription);
    }

    // ─── StatutInscription ───

    [Fact]
    public void StatutInscription_Valide_PasDerreur()
    {
        var command = CreateValidCommand() with { StatutInscription = "Validee" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutInscription);
    }

    [Fact]
    public void StatutInscription_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutInscription = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutInscription);
    }

    [Fact]
    public void StatutInscription_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutInscription = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutInscription);
    }

    // ─── TypeEtablissement (optionnel) ───

    [Fact]
    public void TypeEtablissement_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { TypeEtablissement = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TypeEtablissement);
    }

    [Fact]
    public void TypeEtablissement_Valide_PasDerreur()
    {
        var command = CreateValidCommand() with { TypeEtablissement = "Public" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TypeEtablissement);
    }

    [Theory]
    [InlineData("Public")]
    [InlineData("PriveLaique")]
    [InlineData("PriveFrancoArabe")]
    [InlineData("Communautaire")]
    [InlineData("PriveCatholique")]
    [InlineData("PriveProtestant")]
    public void TypeEtablissement_TousLesTypesValides(string type)
    {
        var command = CreateValidCommand() with { TypeEtablissement = type };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TypeEtablissement);
    }

    [Fact]
    public void TypeEtablissement_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { TypeEtablissement = "Militaire" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypeEtablissement);
    }

    // ─── SerieBac (optionnel) ───

    [Theory]
    [InlineData("SE")]
    [InlineData("SM")]
    [InlineData("SS")]
    [InlineData("FA")]
    public void SerieBac_ToutesLesSeriesValides(string serie)
    {
        var command = CreateValidCommand() with { SerieBac = serie };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.SerieBac);
    }

    [Fact]
    public void SerieBac_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { SerieBac = "XX" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SerieBac);
    }

    // ─── UserId ───

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ─── Helper ───

    private static CreateInscriptionCommand CreateValidCommand()
    {
        return new CreateInscriptionCommand(
            TypeId: 1,
            EleveId: 10,
            ClasseId: 5,
            AnneeScolaireId: 1,
            DateInscription: new DateTime(2025, 9, 1),
            MontantInscription: 150000m,
            EstPaye: false,
            EstRedoublant: false,
            TypeEtablissement: null,
            SerieBac: null,
            StatutInscription: "EnAttente",
            UserId: 1);
    }
}
