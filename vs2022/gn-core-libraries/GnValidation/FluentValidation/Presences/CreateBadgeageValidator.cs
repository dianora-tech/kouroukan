using GnValidation.Commands.Presences;
using FluentValidation;

namespace GnValidation.FluentValidation.Presences;

/// <summary>
/// Validateur pour la creation d'un badgeage.
/// Verifie la reference eleve, la date et l'heure de badgeage,
/// le point d'acces et la methode de badgeage.
/// </summary>
public sealed class CreateBadgeageValidator : BaseEntityValidator<CreateBadgeageCommand>
{
    public CreateBadgeageValidator()
    {
        RuleForRequiredFk(x => x.EleveId, "eleve");

        RuleFor(x => x.DateBadgeage)
            .NotEmpty().WithMessage("La date de badgeage est obligatoire");

        RuleFor(x => x.HeureBadgeage)
            .NotEqual(default(TimeSpan)).WithMessage("L'heure de badgeage est obligatoire");

        RuleForEnum(x => x.PointAcces, ["Entree", "Sortie", "Cantine", "Bibliotheque", "SalleDeSport"], "point d'acces");

        RuleForEnum(x => x.MethodeBadgeage, ["NFC", "QRCode", "Manuel"], "methode de badgeage");
    }
}
