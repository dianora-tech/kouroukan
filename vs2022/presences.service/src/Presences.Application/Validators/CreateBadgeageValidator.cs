using FluentValidation;
using GnValidation.FluentValidation;
using Presences.Application.Commands;

namespace Presences.Application.Validators;

public sealed class CreateBadgeageValidator : BaseEntityValidator<CreateBadgeageCommand>
{
    public CreateBadgeageValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleFor(x => x.DateBadgeage).NotEmpty().WithMessage("La date du badgeage est obligatoire");
        RuleForEnum(x => x.PointAcces, ["Entree", "Sortie", "Cantine", "Biblio"], "Point d'acces");
        RuleForEnum(x => x.MethodeBadgeage, ["NFC", "QRCode", "Manuel"], "Methode de badgeage");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
