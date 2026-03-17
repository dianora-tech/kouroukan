using FluentValidation;
using GnValidation.FluentValidation;
using Support.Application.Commands;

namespace Support.Application.Validators;

public sealed class CreateTicketValidator : BaseEntityValidator<CreateTicketCommand>
{
    public CreateTicketValidator()
    {
        RuleForRequiredFk(x => x.AuteurId, "Auteur");
        RuleForRequiredString(x => x.Sujet, 200, "Sujet");
        RuleForRequiredString(x => x.Contenu, 50000, "Contenu");
        RuleForEnum(x => x.Priorite, ["Basse", "Moyenne", "Haute", "Critique"], "Priorite");
        RuleForEnum(x => x.CategorieTicket, ["Bug", "Question", "Amelioration", "Autre"], "Categorie");
        RuleForOptionalString(x => x.ModuleConcerne, 50, "Module concerne");
        RuleForUrl(x => x.PieceJointeUrl, "Piece jointe URL");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
