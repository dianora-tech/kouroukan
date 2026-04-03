using FluentAssertions;
using FluentValidation.TestHelper;
using ServicesPremium.Application.Commands;
using ServicesPremium.Application.Validators;
using Xunit;

namespace ServicesPremium.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateServiceParentValidator.
/// </summary>
public sealed class CreateServiceParentValidatorTests
{
    private readonly CreateServiceParentValidator _validator;

    public CreateServiceParentValidatorTests()
    {
        _validator = new CreateServiceParentValidator();
    }

    [Fact]
    public void Commande_Valide_PasDerreur()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- TypeId ---

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

    // --- Name ---

    [Fact]
    public void Name_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Name = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Name = new string('A', 201) };

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

    [Fact]
    public void Code_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { Code = new string('C', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    // --- Tarif ---

    [Fact]
    public void Tarif_Negatif_Erreur()
    {
        var command = CreateValidCommand() with { Tarif = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Tarif);
    }

    [Fact]
    public void Tarif_Zero_PasDerreur()
    {
        var command = CreateValidCommand() with { Tarif = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Tarif);
    }

    // --- Periodicite ---

    [Theory]
    [InlineData("Mensuel")]
    [InlineData("Annuel")]
    [InlineData("Unite")]
    public void Periodicite_Valide_PasDerreur(string periodicite)
    {
        var command = CreateValidCommand() with { Periodicite = periodicite };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Periodicite);
    }

    [Fact]
    public void Periodicite_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { Periodicite = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Periodicite);
    }

    [Fact]
    public void Periodicite_Vide_Erreur()
    {
        var command = CreateValidCommand() with { Periodicite = "" };

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

    private static CreateServiceParentCommand CreateValidCommand()
    {
        return new CreateServiceParentCommand(
            TypeId: 1,
            Name: "Service SMS",
            Description: "Alertes SMS",
            Code: "SVC-SMS-001",
            Tarif: 50000m,
            Periodicite: "Mensuel",
            EstActif: true,
            PeriodeEssaiJours: 30,
            TarifDegressif: false,
            UserId: 1);
    }
}
