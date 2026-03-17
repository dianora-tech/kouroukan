using GnValidation.Commands.Communication;
using FluentValidation;

namespace GnValidation.FluentValidation.Communication;

/// <summary>
/// Validateur pour la creation d'une notification.
/// Verifie le nom, les destinataires, le contenu, le canal
/// et le lien d'action optionnel.
/// </summary>
public sealed class CreateNotificationValidator : BaseEntityValidator<CreateNotificationCommand>
{
    public CreateNotificationValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleFor(x => x.DestinatairesIds)
            .NotEmpty().WithMessage("Les destinataires sont obligatoires");

        RuleForRequiredString(x => x.Contenu, 500, "contenu");

        RuleForEnum(x => x.Canal, ["Push", "SMS", "Email", "InApp"], "canal");

        RuleForUrl(x => x.LienAction, "lien action");
    }
}
