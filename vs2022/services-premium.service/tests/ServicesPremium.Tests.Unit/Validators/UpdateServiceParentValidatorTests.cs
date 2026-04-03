using FluentAssertions;
using FluentValidation.TestHelper;
using ServicesPremium.Application.Commands;
using ServicesPremium.Application.Validators;
using Xunit;

namespace ServicesPremium.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour UpdateServiceParentValidator.
/// </summary>
public sealed class UpdateServiceParentValidatorTests
{
    private readonly UpdateServiceParentValidator _validator;

    public UpdateServiceParentValidatorTests()
    {
        _validator = new UpdateServiceParentValidator();
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

    // --- Name ---

    [Fact]
    public void Name_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Name = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // --- Code ---

    [Fact]
    public void Code_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Code = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    // --- Periodicite ---

    [Fact]
    public void Periodicite_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Periodicite = "Trimestriel" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Periodicite);
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

    private static UpdateServiceParentCommand CreateValidCommand()
    {
        return new UpdateServiceParentCommand(
            Id: 1,
            TypeId: 1,
            Name: "Service SMS",
            Description: null,
            Code: "SVC-SMS-001",
            Tarif: 50000m,
            Periodicite: "Mensuel",
            EstActif: true,
            PeriodeEssaiJours: null,
            TarifDegressif: false,
            UserId: 1);
    }
}
