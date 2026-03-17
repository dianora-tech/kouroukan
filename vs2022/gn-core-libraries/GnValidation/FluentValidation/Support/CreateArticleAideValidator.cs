using GnValidation.Commands.Support;
using FluentValidation;

namespace GnValidation.FluentValidation.Support;

/// <summary>
/// Validateur pour la creation d'un article d'aide.
/// Verifie le titre, le contenu, le slug, la categorie,
/// le module concerne et l'ordre d'affichage.
/// </summary>
public sealed class CreateArticleAideValidator : BaseEntityValidator<CreateArticleAideCommand>
{
    public CreateArticleAideValidator()
    {
        RuleForRequiredString(x => x.Titre, 200, "titre");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu est obligatoire");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Le champ slug est obligatoire")
            .MaximumLength(200).WithMessage("Le champ slug ne peut pas depasser 200 caracteres")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Le slug doit etre au format kebab-case (lettres minuscules, chiffres et tirets)");

        RuleForEnum(x => x.Categorie, ["Demarrage", "Utilisation", "FAQ", "Mobile", "Depannage"], "categorie");

        RuleForOptionalString(x => x.ModuleConcerne, 50, "module concerne");

        RuleFor(x => x.Ordre)
            .GreaterThanOrEqualTo(0).WithMessage("L'ordre doit etre superieur ou egal a 0");
    }
}
