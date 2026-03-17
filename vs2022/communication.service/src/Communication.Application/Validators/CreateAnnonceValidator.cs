using FluentValidation;
using GnValidation.FluentValidation;
using Communication.Application.Commands;

namespace Communication.Application.Validators;

public sealed class CreateAnnonceValidator : BaseEntityValidator<CreateAnnonceCommand>
{
    public CreateAnnonceValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForOptionalString(x => x.Description, 500, "Description");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredString(x => x.Contenu, 10000, "Contenu");
        RuleFor(x => x.DateDebut).NotEmpty().WithMessage("La date de debut est obligatoire");
        RuleForEnum(x => x.CibleAudience, new[] { "Tous", "Parents", "Enseignants", "Eleves" }, "Cible audience");
        RuleFor(x => x.Priorite).GreaterThanOrEqualTo(1).WithMessage("La priorite doit etre superieure ou egale a 1");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
