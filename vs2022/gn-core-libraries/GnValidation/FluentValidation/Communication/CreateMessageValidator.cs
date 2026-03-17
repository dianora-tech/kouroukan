using GnValidation.Commands.Communication;
using FluentValidation;

namespace GnValidation.FluentValidation.Communication;

/// <summary>
/// Validateur pour la creation d'un message.
/// Verifie l'expediteur, le destinataire optionnel, le sujet, le contenu
/// et le groupe destinataire optionnel.
/// </summary>
public sealed class CreateMessageValidator : BaseEntityValidator<CreateMessageCommand>
{
    public CreateMessageValidator()
    {
        RuleForRequiredFk(x => x.ExpediteurId, "expediteur");

        RuleFor(x => x.DestinataireId)
            .GreaterThan(0).WithMessage("L'identifiant du destinataire doit etre superieur a 0")
            .When(x => x.DestinataireId.HasValue);

        RuleForRequiredString(x => x.Sujet, 200, "sujet");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu est obligatoire");

        RuleForOptionalString(x => x.GroupeDestinataire, 100, "groupe destinataire");
    }
}
