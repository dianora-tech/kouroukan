using FluentAssertions;
using FluentValidation.TestHelper;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Validators;
using Xunit;

namespace Inscriptions.Tests.Unit.Validators;

/// <summary>
/// Tests unitaires pour CreateDossierAdmissionValidator.
/// </summary>
public sealed class CreateDossierAdmissionValidatorTests
{
    private readonly CreateDossierAdmissionValidator _validator;

    public CreateDossierAdmissionValidatorTests()
    {
        _validator = new CreateDossierAdmissionValidator();
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

    // ─── EleveId ───

    [Fact]
    public void EleveId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { EleveId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EleveId);
    }

    // ─── AnneeScolaireId ───

    [Fact]
    public void AnneeScolaireId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { AnneeScolaireId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AnneeScolaireId);
    }

    // ─── StatutDossier ───

    [Theory]
    [InlineData("Prospect")]
    [InlineData("PreInscrit")]
    [InlineData("EnEtude")]
    [InlineData("Convoque")]
    [InlineData("Admis")]
    [InlineData("Refuse")]
    [InlineData("ListeAttente")]
    public void StatutDossier_TousLesStatutsValides(string statut)
    {
        var command = CreateValidCommand() with { StatutDossier = statut };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.StatutDossier);
    }

    [Fact]
    public void StatutDossier_Invalide_Erreur()
    {
        var command = CreateValidCommand() with { StatutDossier = "INVALIDE" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutDossier);
    }

    [Fact]
    public void StatutDossier_Vide_Erreur()
    {
        var command = CreateValidCommand() with { StatutDossier = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StatutDossier);
    }

    // ─── EtapeActuelle ───

    [Fact]
    public void EtapeActuelle_Vide_Erreur()
    {
        var command = CreateValidCommand() with { EtapeActuelle = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EtapeActuelle);
    }

    [Fact]
    public void EtapeActuelle_TropLong_Erreur()
    {
        var command = CreateValidCommand() with { EtapeActuelle = new string('A', 51) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EtapeActuelle);
    }

    // ─── DateDemande ───

    [Fact]
    public void DateDemande_Default_Erreur()
    {
        var command = CreateValidCommand() with { DateDemande = default };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateDemande);
    }

    // ─── MotifRefus (optionnel) ───

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

    // ─── UserId ───

    [Fact]
    public void UserId_Zero_Erreur()
    {
        var command = CreateValidCommand() with { UserId = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ─── Helper ───

    private static CreateDossierAdmissionCommand CreateValidCommand()
    {
        return new CreateDossierAdmissionCommand(
            TypeId: 1,
            EleveId: 10,
            AnneeScolaireId: 1,
            StatutDossier: "EnEtude",
            EtapeActuelle: "DepotDossier",
            DateDemande: new DateTime(2025, 6, 1),
            DateDecision: null,
            MotifRefus: null,
            ScoringInterne: null,
            Commentaires: null,
            ResponsableAdmissionId: null,
            UserId: 1);
    }
}
