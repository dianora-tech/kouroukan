using FluentValidation;
using GnValidation.FluentValidation;
using Inscriptions.Application.Commands;

namespace Inscriptions.Application.Validators;

public sealed class CreateDossierAdmissionValidator : BaseEntityValidator<CreateDossierAdmissionCommand>
{
    public CreateDossierAdmissionValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleForRequiredFk(x => x.AnneeScolaireId, "Annee scolaire");
        RuleForEnum(x => x.StatutDossier, ["Prospect", "PreInscrit", "EnEtude", "Convoque", "Admis", "Refuse", "ListeAttente"], "Statut dossier");
        RuleForRequiredString(x => x.EtapeActuelle, 50, "Etape actuelle");
        RuleFor(x => x.DateDemande).NotEmpty().WithMessage("La date de demande est obligatoire");
        RuleForOptionalString(x => x.MotifRefus, 500, "Motif de refus");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
