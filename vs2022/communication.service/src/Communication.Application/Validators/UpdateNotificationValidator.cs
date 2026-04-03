using FluentValidation;
using GnValidation.FluentValidation;
using Communication.Application.Commands;

namespace Communication.Application.Validators;

public sealed class UpdateNotificationValidator : BaseEntityValidator<UpdateNotificationCommand>
{
    public UpdateNotificationValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredString(x => x.DestinatairesIds, 10000, "Destinataires");
        RuleForRequiredString(x => x.Contenu, 500, "Contenu");
        RuleForEnum(x => x.Canal, new[] { "Push", "SMS", "Email", "InApp" }, "Canal");
        RuleForUrl(x => x.LienAction, "Lien action");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
