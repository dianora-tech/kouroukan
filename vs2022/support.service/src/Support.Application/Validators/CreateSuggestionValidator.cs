using FluentValidation;
using GnValidation.FluentValidation;
using Support.Application.Commands;

namespace Support.Application.Validators;

public sealed class CreateSuggestionValidator : BaseEntityValidator<CreateSuggestionCommand>
{
    public CreateSuggestionValidator()
    {
        RuleForRequiredFk(x => x.AuteurId, "Auteur");
        RuleForRequiredString(x => x.Titre, 200, "Titre");
        RuleForRequiredString(x => x.Contenu, 50000, "Contenu");
        RuleForOptionalString(x => x.ModuleConcerne, 50, "Module concerne");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
