using FluentAssertions;
using FluentValidation.TestHelper;
using Documents.Application.Commands;
using Documents.Application.Validators;
using Xunit;

namespace Documents.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateSignatureValidator.
/// </summary>
public sealed class CreateSignatureValidatorTests
{
    private readonly CreateSignatureValidator _validator;

    public CreateSignatureValidatorTests()
    {
        _validator = new CreateSignatureValidator();
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

    // ─── DocumentGenereId ───

    [Fact]
    public void DocumentGenereId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { DocumentGenereId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DocumentGenereId);
    }

    [Fact]
    public void DocumentGenereId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { DocumentGenereId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DocumentGenereId);
    }

    // ─── SignataireId ───

    [Fact]
    public void SignataireId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { SignataireId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SignataireId);
    }

    [Fact]
    public void SignataireId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { SignataireId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SignataireId);
    }

    // ─── OrdreSignature ───

    [Fact]
    public void OrdreSignature_Zero_Erreur()
    {
        var command = CreateValidCommand() with { OrdreSignature = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.OrdreSignature);
    }

    [Fact]
    public void OrdreSignature_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { OrdreSignature = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.OrdreSignature);
    }

    [Fact]
    public void OrdreSignature_Positif_PasDerreur()
    {
        var command = CreateValidCommand() with { OrdreSignature = 5 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.OrdreSignature);
    }

    // ─── StatutSignature ───

    [Theory]
    [InlineData("EnAttente")]
    [InlineData("Signe")]
    [InlineData("Refuse")]
    [InlineData("Delegue")]
    public void StatutSignature_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutSignature = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutSignature);
    }

    [Fact]
    public void StatutSignature_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutSignature = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutSignature);
    }

    [Fact]
    public void StatutSignature_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutSignature = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutSignature);
    }

    // ─── NiveauSignature ───

    [Theory]
    [InlineData("Basique")]
    [InlineData("Avancee")]
    public void NiveauSignature_Valide_PasDerreur(string niveau)
    {
        var command = CreateValidCommand() with { NiveauSignature = niveau };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NiveauSignature);
    }

    [Fact]
    public void NiveauSignature_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { NiveauSignature = "Expert" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NiveauSignature);
    }

    [Fact]
    public void NiveauSignature_Vide_Erreur()
    {
        var command = CreateValidCommand() with { NiveauSignature = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NiveauSignature);
    }

    // ─── MotifRefus (optionnel, max 500) ───

    [Fact]
    public void MotifRefus_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { MotifRefus = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MotifRefus);
    }

    [Fact]
    public void MotifRefus_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { MotifRefus = new string('A', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MotifRefus);
    }

    [Fact]
    public void MotifRefus_LongueurMax_PasDerreur()
    {
        var command = CreateValidCommand() with { MotifRefus = new string('A', 500) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MotifRefus);
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

    private static CreateSignatureCommand CreateValidCommand()
    {
        return new CreateSignatureCommand(
            TypeId: 1,
            DocumentGenereId: 10,
            SignataireId: 5,
            OrdreSignature: 1,
            DateSignature: null,
            StatutSignature: "EnAttente",
            NiveauSignature: "Basique",
            MotifRefus: null,
            EstValidee: false,
            UserId: 1);
    }
}
