using FluentAssertions;
using FluentValidation.TestHelper;
using ServicesPremium.Application.Commands;
using ServicesPremium.Application.Validators;
using Xunit;

namespace ServicesPremium.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateSouscriptionValidator.
/// </summary>
public sealed class CreateSouscriptionValidatorTests
{
    private readonly CreateSouscriptionValidator _validator;

    public CreateSouscriptionValidatorTests()
    {
        _validator = new CreateSouscriptionValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- ServiceParentId ---

    [Fact]
    public void ServiceParentId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ServiceParentId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ServiceParentId);
    }

    // --- ParentId ---

    [Fact]
    public void ParentId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { ParentId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ParentId);
    }

    // --- DateDebut ---

    [Fact]
    public void DateDebut_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateDebut = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateDebut);
    }

    // --- StatutSouscription ---

    [Theory]
    [InlineData("Active")]
    [InlineData("Expiree")]
    [InlineData("Resiliee")]
    [InlineData("Essai")]
    public void StatutSouscription_Valide_PasDerreur(string statut)
    {
        var command = CreateValidCommand() with { StatutSouscription = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutSouscription);
    }

    [Fact]
    public void StatutSouscription_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutSouscription = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutSouscription);
    }

    [Fact]
    public void StatutSouscription_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutSouscription = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutSouscription);
    }

    // --- MontantPaye ---

    [Fact]
    public void MontantPaye_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { MontantPaye = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MontantPaye);
    }

    [Fact]
    public void MontantPaye_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { MontantPaye = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MontantPaye);
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

    private static CreateSouscriptionCommand CreateValidCommand()
    {
        return new CreateSouscriptionCommand(
            Name: "Souscription SMS",
            Description: null,
            ServiceParentId: 10,
            ParentId: 5,
            DateDebut: new DateTime(2025, 9, 1),
            DateFin: null,
            StatutSouscription: "Active",
            MontantPaye: 50000m,
            RenouvellementAuto: true,
            DateProchainRenouvellement: null,
            UserId: 1);
    }
}
