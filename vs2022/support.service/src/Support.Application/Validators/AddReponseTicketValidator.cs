using FluentValidation;
using GnValidation.FluentValidation;
using Support.Application.Commands;

namespace Support.Application.Validators;

public sealed class AddReponseTicketValidator : BaseEntityValidator<AddReponseTicketCommand>
{
    public AddReponseTicketValidator()
    {
        RuleForRequiredFk(x => x.TicketId, "Ticket");
        RuleForRequiredFk(x => x.AuteurId, "Auteur");
        RuleForRequiredString(x => x.Contenu, 50000, "Contenu");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
