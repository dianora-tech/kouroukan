using FluentValidation;
using GnValidation.FluentValidation;
using Bde.Application.Commands;

namespace Bde.Application.Validators;

public sealed class CreateEvenementValidator : BaseEntityValidator<CreateEvenementCommand>
{
    public CreateEvenementValidator()
    {
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
