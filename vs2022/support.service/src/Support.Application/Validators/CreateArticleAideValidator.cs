using FluentValidation;
using GnValidation.FluentValidation;
using Support.Application.Commands;

namespace Support.Application.Validators;

public sealed class CreateArticleAideValidator : BaseEntityValidator<CreateArticleAideCommand>
{
    public CreateArticleAideValidator()
    {
        RuleForRequiredString(x => x.Titre, 200, "Titre");
        RuleForRequiredString(x => x.Contenu, 50000, "Contenu");
        RuleForRequiredString(x => x.Slug, 200, "Slug");
        RuleForRequiredString(x => x.Categorie, 100, "Categorie");
        RuleForOptionalString(x => x.ModuleConcerne, 50, "Module concerne");
        RuleFor(x => x.Ordre).GreaterThanOrEqualTo(0).WithMessage("L'ordre doit etre positif ou nul.");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
