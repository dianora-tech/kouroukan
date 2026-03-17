using GnValidation.Commands.Communication;
using FluentValidation;

namespace GnValidation.FluentValidation.Communication;

/// <summary>
/// Validateur pour la creation d'une annonce.
/// Verifie le nom, le contenu, les dates de debut et fin,
/// la cible audience et la priorite.
/// </summary>
public sealed class CreateAnnonceValidator : BaseEntityValidator<CreateAnnonceCommand>
{
    public CreateAnnonceValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu est obligatoire");

        RuleFor(x => x.DateDebut)
            .NotEmpty().WithMessage("La date de debut est obligatoire");

        RuleFor(x => x.DateFin)
            .GreaterThan(x => x.DateDebut).WithMessage("La date de fin doit etre posterieure a la date de debut")
            .When(x => x.DateFin.HasValue);

        RuleForEnum(x => x.CibleAudience, ["Tous", "Parents", "Enseignants", "Eleves"], "cible audience");

        RuleFor(x => x.Priorite)
            .GreaterThan(0).WithMessage("La priorite doit etre superieure a 0");
    }
}
