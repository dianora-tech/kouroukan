using GnValidation.Commands.Support;
using FluentValidation;

namespace GnValidation.FluentValidation.Support;

/// <summary>
/// Validateur pour la creation d'une reponse a un ticket.
/// Verifie le ticket, l'auteur et le contenu de la reponse.
/// </summary>
public sealed class CreateReponseTicketValidator : BaseEntityValidator<CreateReponseTicketCommand>
{
    public CreateReponseTicketValidator()
    {
        RuleForRequiredFk(x => x.TicketId, "ticket");

        RuleForRequiredFk(x => x.AuteurId, "auteur");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu de la reponse est obligatoire");
    }
}
