using FluentValidation;
using GnValidation.FluentValidation;
using Support.Application.Commands;

namespace Support.Application.Validators;

public sealed class UpdateTicketValidator : BaseEntityValidator<UpdateTicketCommand>
{
    public UpdateTicketValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire.");
        RuleForRequiredFk(x => x.AuteurId, "Auteur");
        RuleForRequiredString(x => x.Sujet, 200, "Sujet");
        RuleForRequiredString(x => x.Contenu, 50000, "Contenu");
        RuleForEnum(x => x.Priorite, ["Basse", "Moyenne", "Haute", "Critique"], "Priorite");
        RuleForEnum(x => x.StatutTicket, ["Ouvert", "EnCours", "EnAttente", "Resolu", "Ferme"], "Statut");
        RuleForEnum(x => x.CategorieTicket, ["Bug", "Question", "Amelioration", "Autre"], "Categorie");
        RuleForOptionalString(x => x.ModuleConcerne, 50, "Module concerne");
        RuleForUrl(x => x.PieceJointeUrl, "Piece jointe URL");
        RuleFor(x => x.NoteSatisfaction)
            .InclusiveBetween(1, 5)
            .When(x => x.NoteSatisfaction.HasValue)
            .WithMessage("La note de satisfaction doit etre entre 1 et 5.");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
