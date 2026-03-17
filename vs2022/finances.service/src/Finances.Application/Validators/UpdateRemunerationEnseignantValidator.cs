using Finances.Application.Commands;
using FluentValidation;
using GnValidation.FluentValidation;

namespace Finances.Application.Validators;

public sealed class UpdateRemunerationEnseignantValidator : BaseEntityValidator<UpdateRemunerationEnseignantCommand>
{
    public UpdateRemunerationEnseignantValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.EnseignantId, "Enseignant");
        RuleFor(x => x.Mois).InclusiveBetween(1, 12).WithMessage("Le mois doit etre compris entre 1 et 12");
        RuleFor(x => x.Annee).GreaterThanOrEqualTo(2000).WithMessage("L'annee doit etre superieure ou egale a 2000");
        RuleForEnum(x => x.ModeRemuneration,
            ["Forfait", "Heures", "Mixte"], "Mode de remuneration");
        RuleForEnum(x => x.StatutPaiement,
            ["EnAttente", "Valide", "Paye"], "Statut du paiement");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
