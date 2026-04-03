using FluentAssertions;
using FluentValidation.TestHelper;
using Finances.Application.Commands;
using Finances.Application.Validators;
using Xunit;

namespace Finances.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateRemunerationEnseignantValidator.
/// </summary>
public sealed class CreateRemunerationEnseignantValidatorTests
{
    private readonly CreateRemunerationEnseignantValidator _validator;

    public CreateRemunerationEnseignantValidatorTests()
    {
        _validator = new CreateRemunerationEnseignantValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- EnseignantId ---

    [Fact]
    public void EnseignantId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EnseignantId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EnseignantId);
    }

    [Fact]
    public void EnseignantId_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { EnseignantId = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EnseignantId);
    }

    // --- Mois ---

    [Fact]
    public void Mois_Zero_Erreur()
    {
        var command = CreateValidCommand() with { Mois = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Mois);
    }

    [Fact]
    public void Mois_Treize_Erreur()
    {
        var command = CreateValidCommand() with { Mois = 13 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Mois);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void Mois_Valide_PasDerreur(int mois)
    {
        var command = CreateValidCommand() with { Mois = mois };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Mois);
    }

    // --- Annee ---

    [Fact]
    public void Annee_InferieureA2000_Erreur()
    {
        var command = CreateValidCommand() with { Annee = 1999 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Annee);
    }

    [Fact]
    public void Annee_2000_PasDerreur()
    {
        var command = CreateValidCommand() with { Annee = 2000 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Annee);
    }

    // --- ModeRemuneration ---

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

    // --- StatutPaiement ---

    [Theory]
    [InlineData("EnAttente")]
    [InlineData("Valide")]
    [InlineData("Paye")]
    public void StatutPaiement_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutPaiement = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutPaiement);
    }

    [Fact]
    public void StatutPaiement_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutPaiement = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutPaiement);
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

    private static CreateRemunerationEnseignantCommand CreateValidCommand()
    {
        return new CreateRemunerationEnseignantCommand(
            EnseignantId: 10,
            Mois: 3,
            Annee: 2024,
            ModeRemuneration: "Forfait",
            MontantForfait: 500000m,
            NombreHeures: null,
            TauxHoraire: null,
            StatutPaiement: "EnAttente",
            DateValidation: null,
            ValidateurId: null,
            UserId: 1);
    }
}
