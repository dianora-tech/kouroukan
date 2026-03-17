using FluentValidation;
using GnValidation.FluentValidation;
using Communication.Application.Commands;

namespace Communication.Application.Validators;

public sealed class UpdateMessageValidator : BaseEntityValidator<UpdateMessageCommand>
{
    public UpdateMessageValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForOptionalString(x => x.Description, 500, "Description");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.ExpediteurId, "Expediteur");
        RuleForRequiredString(x => x.Sujet, 200, "Sujet");
        RuleForRequiredString(x => x.Contenu, 10000, "Contenu");
        RuleForOptionalString(x => x.GroupeDestinataire, 100, "Groupe destinataire");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
