using GnValidation.Commands.Inscriptions;
using FluentValidation;

namespace GnValidation.FluentValidation.Inscriptions;

/// <summary>
/// Validateur pour la creation d'un dossier d'admission.
/// Verifie les references eleve et annee scolaire, le statut, les dates et le scoring.
/// </summary>
public sealed class CreateDossierAdmissionValidator : BaseEntityValidator<CreateDossierAdmissionCommand>
{
    public CreateDossierAdmissionValidator()
    {
        RuleForRequiredFk(x => x.EleveId, "eleve");

        RuleForRequiredFk(x => x.AnneeScolaireId, "annee scolaire");

        RuleForEnum(x => x.StatutDossier, ["Prospect", "PreInscrit", "EnEtude", "Convoque", "Admis", "Refuse", "ListeAttente"], "statut dossier");

        RuleForRequiredString(x => x.EtapeActuelle, 50, "etape actuelle");

        RuleFor(x => x.DateDemande)
            .NotEmpty().WithMessage("La date de demande est obligatoire");

        RuleForOptionalString(x => x.MotifRefus, 500, "motif de refus");

        RuleFor(x => x.ScoringInterne)
            .InclusiveBetween(0, 999.99m).WithMessage("Le scoring interne doit etre entre 0 et 999.99")
            .When(x => x.ScoringInterne.HasValue);

        // Commentaires : TEXT nullable, pas de validation

        RuleFor(x => x.ResponsableAdmissionId)
            .GreaterThan(0).WithMessage("L'identifiant du responsable d'admission doit etre superieur a 0")
            .When(x => x.ResponsableAdmissionId.HasValue);
    }
}
