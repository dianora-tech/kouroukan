using GnValidation.Commands.ServicesPremium;
using FluentValidation;

namespace GnValidation.FluentValidation.ServicesPremium;

/// <summary>
/// Validateur pour la creation d'un service parent.
/// Verifie le nom, le code, le tarif, la periodicite
/// et la periode d'essai optionnelle.
/// </summary>
public sealed class CreateServiceParentValidator : BaseEntityValidator<CreateServiceParentCommand>
{
    public CreateServiceParentValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredString(x => x.Code, 50, "code");

        RuleForMoney(x => x.Tarif, "tarif");

        RuleForEnum(x => x.Periodicite, ["Mensuel", "Annuel", "Unite"], "periodicite");

        RuleFor(x => x.PeriodeEssaiJours)
            .GreaterThan(0).WithMessage("La periode d'essai doit etre superieure a 0 jours")
            .When(x => x.PeriodeEssaiJours.HasValue);
    }
}
