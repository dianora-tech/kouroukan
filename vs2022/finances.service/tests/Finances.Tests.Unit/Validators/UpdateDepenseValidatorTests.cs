using FluentAssertions;
using FluentValidation.TestHelper;
using Finances.Application.Commands;
using Finances.Application.Validators;
using Xunit;

namespace Finances.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateDepenseValidator.
/// </summary>
public sealed class UpdateDepenseValidatorTests
{
    private readonly UpdateDepenseValidator _validator;

    public UpdateDepenseValidatorTests()
    {
        _validator = new UpdateDepenseValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- Id ---

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

    // --- TypeId ---

    [Fact]
    public void TypeId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { TypeId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TypeId);
    }

    // --- Montant ---

    [Fact]
    public void Montant_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Montant = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Montant);
    }

    // --- Categorie ---

    [Fact]
    public void Categorie_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Categorie = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Categorie);
    }

    // --- StatutDepense ---

    [Fact]
    public void StatutDepense_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutDepense = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutDepense);
    }

    // --- DateDemande ---

    [Fact]
    public void DateDemande_Vide_Erreur()
    {
        var command = CreateValidCommand() with { DateDemande = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateDemande);
    }

    // --- NumeroJustificatif ---

    [Fact]
    public void NumeroJustificatif_Vide_Erreur()
    {
        var command = CreateValidCommand() with { NumeroJustificatif = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NumeroJustificatif);
    }

    // --- UserId ---

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // --- Helper ---

    private static UpdateDepenseCommand CreateValidCommand()
    {
        return new UpdateDepenseCommand(
            Id: 1,
            TypeId: 1,
            Montant: 50000m,
            MotifDepense: "Achat fournitures bureau",
            Categorie: "Fournitures",
            BeneficiaireNom: "Papeterie Centrale",
            BeneficiaireTelephone: "622000000",
            BeneficiaireNIF: "NIF-001",
            StatutDepense: "Demande",
            DateDemande: DateTime.UtcNow,
            DateValidation: null,
            ValidateurId: null,
            PieceJointeUrl: null,
            NumeroJustificatif: "JUST-2024-001",
            UserId: 1);
    }
}
