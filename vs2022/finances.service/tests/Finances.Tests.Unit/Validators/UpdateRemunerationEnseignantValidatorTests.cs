using FluentAssertions;
using FluentValidation.TestHelper;
using Finances.Application.Commands;
using Finances.Application.Validators;
using Xunit;

namespace Finances.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateRemunerationEnseignantValidator.
/// </summary>
public sealed class UpdateRemunerationEnseignantValidatorTests
{
    private readonly UpdateRemunerationEnseignantValidator _validator;

    public UpdateRemunerationEnseignantValidatorTests()
    {
        _validator = new UpdateRemunerationEnseignantValidator();
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

    // --- EnseignantId ---

    [Fact]
    public void EnseignantId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EnseignantId = 0 };

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

    // --- Annee ---

    [Fact]
    public void Annee_InferieureA2000_Erreur()
    {
        var command = CreateValidCommand() with { Annee = 1999 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Annee);
    }

    // --- ModeRemuneration ---

    [Fact]
    public void ModeRemuneration_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { ModeRemuneration = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ModeRemuneration);
    }

    // --- StatutPaiement ---

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

    private static UpdateRemunerationEnseignantCommand CreateValidCommand()
    {
        return new UpdateRemunerationEnseignantCommand(
            Id: 1,
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
