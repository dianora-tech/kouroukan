using FluentValidation;
using GnValidation.FluentValidation;
using Support.Application.Commands;

namespace Support.Application.Validators;

public sealed class UpdateSuggestionValidator : BaseEntityValidator<UpdateSuggestionCommand>
{
    public UpdateSuggestionValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire.");
        RuleForRequiredFk(x => x.AuteurId, "Auteur");
        RuleForRequiredString(x => x.Titre, 200, "Titre");
        RuleForRequiredString(x => x.Contenu, 50000, "Contenu");
        RuleForOptionalString(x => x.ModuleConcerne, 50, "Module concerne");
        RuleForEnum(x => x.StatutSuggestion, ["Soumise", "EnRevue", "Acceptee", "Planifiee", "Realisee", "Rejetee"], "Statut suggestion");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
