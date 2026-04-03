using FluentAssertions;
using FluentValidation.TestHelper;
using Documents.Application.Commands;
using Documents.Application.Validators;
using Xunit;

namespace Documents.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateDocumentGenereValidator.
/// </summary>
public sealed class CreateDocumentGenereValidatorTests
{
    private readonly CreateDocumentGenereValidator _validator;

    public CreateDocumentGenereValidatorTests()
    {
        _validator = new CreateDocumentGenereValidator();
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

    // ─── ModeleDocumentId ───

    [Fact]
    public void ModeleDocumentId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ModeleDocumentId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ModeleDocumentId);
    }

    [Fact]
    public void ModeleDocumentId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { ModeleDocumentId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ModeleDocumentId);
    }

    // ─── DonneesJson ───

    [Fact]
    public void DonneesJson_Vide_Erreur()
    {
        var command = CreateValidCommand() with { DonneesJson = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DonneesJson);
    }

    [Fact]
    public void DonneesJson_Null_Erreur()
    {
        var command = CreateValidCommand() with { DonneesJson = null! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DonneesJson);
    }

    // ─── DateGeneration ───

    [Fact]
    public void DateGeneration_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateGeneration = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateGeneration);
    }

    // ─── StatutSignature ───

    [Theory]
    [InlineData("EnAttente")]
    [InlineData("EnCours")]
    [InlineData("Signe")]
    [InlineData("Refuse")]
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

    // ─── CheminFichier (optionnel URL) ───

    [Fact]
    public void CheminFichier_Null_PasDerreur()
    {
        var command = CreateValidCommand() with { CheminFichier = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CheminFichier);
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

    private static CreateDocumentGenereCommand CreateValidCommand()
    {
        return new CreateDocumentGenereCommand(
            TypeId: 1,
            ModeleDocumentId: 10,
            EleveId: 5,
            EnseignantId: null,
            DonneesJson: "{\"nom\":\"Test\"}",
            DateGeneration: new DateTime(2025, 6, 15),
            StatutSignature: "EnAttente",
            CheminFichier: "/documents/test.pdf",
            UserId: 1);
    }
}
