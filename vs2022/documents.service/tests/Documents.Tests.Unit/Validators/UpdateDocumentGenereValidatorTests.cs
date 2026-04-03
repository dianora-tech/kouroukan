using FluentAssertions;
using FluentValidation.TestHelper;
using Documents.Application.Commands;
using Documents.Application.Validators;
using Xunit;

namespace Documents.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateDocumentGenereValidator.
/// </summary>
public sealed class UpdateDocumentGenereValidatorTests
{
    private readonly UpdateDocumentGenereValidator _validator;

    public UpdateDocumentGenereValidatorTests()
    {
        _validator = new UpdateDocumentGenereValidator();
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

    // ─── TypeId ───

    [Fact]
    public void TypeId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = 0 };

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

    // ─── DonneesJson ───

    [Fact]
    public void DonneesJson_Vide_Erreur()
    {
        var command = CreateValidCommand() with { DonneesJson = "" };

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

    // ─── UserId ───

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ─── Helper ───

    private static UpdateDocumentGenereCommand CreateValidCommand()
    {
        return new UpdateDocumentGenereCommand(
            Id: 1,
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
