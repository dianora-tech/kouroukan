using GnValidation.Commands.Support;
using FluentValidation;

namespace GnValidation.FluentValidation.Support;

/// <summary>
/// Validateur pour la creation d'une suggestion.
/// Verifie l'auteur, le titre, le contenu,
/// le module concerne et le statut de la suggestion.
/// </summary>
public sealed class CreateSuggestionValidator : BaseEntityValidator<CreateSuggestionCommand>
{
    public CreateSuggestionValidator()
    {
        RuleForRequiredFk(x => x.AuteurId, "auteur");

        RuleForRequiredString(x => x.Titre, 200, "titre");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu est obligatoire");

        RuleForOptionalString(x => x.ModuleConcerne, 50, "module concerne");

        RuleForEnum(x => x.StatutSuggestion, ["Soumise", "EnRevue", "Acceptee", "Planifiee", "Realisee", "Rejetee"], "statut suggestion");
    }
}
