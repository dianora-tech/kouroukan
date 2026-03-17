using GnValidation.Commands.Inscriptions;
using FluentValidation;

namespace GnValidation.FluentValidation.Inscriptions;

/// <summary>
/// Validateur pour la creation d'une inscription.
/// Verifie les references eleve, classe et annee scolaire, le montant et les statuts.
/// </summary>
public sealed class CreateInscriptionValidator : BaseEntityValidator<CreateInscriptionCommand>
{
    public CreateInscriptionValidator()
    {
        RuleForRequiredFk(x => x.EleveId, "eleve");

        RuleForRequiredFk(x => x.ClasseId, "classe");

        RuleForRequiredFk(x => x.AnneeScolaireId, "annee scolaire");

        RuleFor(x => x.DateInscription)
            .NotEmpty().WithMessage("La date d'inscription est obligatoire");

        RuleForMoney(x => x.MontantInscription, "montant d'inscription");

        RuleForOptionalEnum(x => x.TypeEtablissement, ["Public", "PriveLaique", "PriveFrancoArabe", "Communautaire", "PriveCatholique", "PriveProtestant"], "type d'etablissement");

        RuleForOptionalEnum(x => x.SerieBac, ["SE", "SM", "SS", "FA"], "serie baccalaureat");

        RuleForEnum(x => x.StatutInscription, ["EnAttente", "Validee", "Annulee"], "statut inscription");
    }
}
