using FluentValidation;
using GnValidation.FluentValidation;
using Presences.Application.Commands;

namespace Presences.Application.Validators;

public sealed class UpdateBadgeageValidator : BaseEntityValidator<UpdateBadgeageCommand>
{
    public UpdateBadgeageValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleFor(x => x.DateBadgeage).NotEmpty().WithMessage("La date du badgeage est obligatoire");
        RuleForEnum(x => x.PointAcces, ["Entree", "Sortie", "Cantine", "Biblio"], "Point d'acces");
        RuleForEnum(x => x.MethodeBadgeage, ["NFC", "QRCode", "Manuel"], "Methode de badgeage");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
