using FluentAssertions;
using FluentValidation.TestHelper;
using ServicesPremium.Application.Commands;
using ServicesPremium.Application.Validators;
using Xunit;

namespace ServicesPremium.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateSouscriptionValidator.
/// </summary>
public sealed class UpdateSouscriptionValidatorTests
{
    private readonly UpdateSouscriptionValidator _validator;

    public UpdateSouscriptionValidatorTests()
    {
        _validator = new UpdateSouscriptionValidator();
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

    [Fact]
    public void StatutSouscription_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutSouscription = "Inconnue" };

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

    // --- UserId ---

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // --- Helper ---

    private static UpdateSouscriptionCommand CreateValidCommand()
    {
        return new UpdateSouscriptionCommand(
            Id: 1,
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
