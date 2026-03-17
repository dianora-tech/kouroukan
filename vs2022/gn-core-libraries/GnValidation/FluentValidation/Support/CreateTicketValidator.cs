using GnValidation.Commands.Support;
using FluentValidation;

namespace GnValidation.FluentValidation.Support;

/// <summary>
/// Validateur pour la creation d'un ticket de support.
/// Verifie l'auteur, le sujet, le contenu, la priorite,
/// le statut, la categorie et le module concerne.
/// </summary>
public sealed class CreateTicketValidator : BaseEntityValidator<CreateTicketCommand>
{
    public CreateTicketValidator()
    {
        RuleForRequiredFk(x => x.AuteurId, "auteur");

        RuleForRequiredString(x => x.Sujet, 200, "sujet");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu est obligatoire");

        RuleForEnum(x => x.Priorite, ["Basse", "Moyenne", "Haute", "Critique"], "priorite");

        RuleForEnum(x => x.StatutTicket, ["Ouvert", "EnCours", "EnAttente", "Resolu", "Ferme"], "statut ticket");

        RuleForEnum(x => x.CategorieTicket, ["Bug", "Question", "Amelioration", "Autre"], "categorie ticket");

        RuleForOptionalString(x => x.ModuleConcerne, 50, "module concerne");
    }
}
