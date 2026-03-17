using FluentValidation;
using GnValidation.FluentValidation;
using Bde.Application.Commands;

namespace Bde.Application.Validators;

public sealed class UpdateEvenementValidator : BaseEntityValidator<UpdateEvenementCommand>
{
    public UpdateEvenementValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForOptionalString(x => x.Description, 1000, "Description");
        RuleForRequiredFk(x => x.AssociationId, "Association");
        RuleFor(x => x.DateEvenement).NotEmpty().WithMessage("La date de l'evenement est obligatoire");
        RuleForRequiredString(x => x.Lieu, 200, "Lieu");
        RuleForEnum(x => x.StatutEvenement, ["Planifie", "Valide", "EnCours", "Termine", "Annule"], "Statut evenement");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
