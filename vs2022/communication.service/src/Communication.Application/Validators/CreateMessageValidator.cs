using FluentValidation;
using GnValidation.FluentValidation;
using Communication.Application.Commands;

namespace Communication.Application.Validators;

public sealed class CreateMessageValidator : BaseEntityValidator<CreateMessageCommand>
{
    public CreateMessageValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.ExpediteurId, "Expediteur");
        RuleForRequiredString(x => x.Sujet, 200, "Sujet");
        RuleForRequiredString(x => x.Contenu, 10000, "Contenu");
        RuleForOptionalString(x => x.GroupeDestinataire, 100, "Groupe destinataire");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
