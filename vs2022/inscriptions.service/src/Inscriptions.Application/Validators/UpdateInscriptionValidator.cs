using FluentValidation;
using GnValidation.FluentValidation;
using Inscriptions.Application.Commands;

namespace Inscriptions.Application.Validators;

public sealed class UpdateInscriptionValidator : BaseEntityValidator<UpdateInscriptionCommand>
{
    public UpdateInscriptionValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleForRequiredFk(x => x.ClasseId, "Classe");
        RuleForRequiredFk(x => x.AnneeScolaireId, "Annee scolaire");
        RuleFor(x => x.DateInscription).NotEmpty().WithMessage("La date d'inscription est obligatoire");
        RuleForMoney(x => x.MontantInscription, "Montant inscription");
        RuleForOptionalEnum(x => x.TypeEtablissement, ["Public", "PriveLaique", "PriveFrancoArabe", "Communautaire", "PriveCatholique", "PriveProtestant"], "Type etablissement");
        RuleForOptionalEnum(x => x.SerieBac, ["SE", "SM", "SS", "FA"], "Serie bac");
        RuleForEnum(x => x.StatutInscription, ["EnAttente", "Validee", "Annulee"], "Statut inscription");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
