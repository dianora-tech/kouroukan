using GnValidation.Commands.ServicesPremium;
using FluentValidation;

namespace GnValidation.FluentValidation.ServicesPremium;

/// <summary>
/// Validateur pour la creation d'une souscription.
/// Verifie le service parent, le parent, les dates de debut et fin,
/// le statut de souscription et le montant paye.
/// </summary>
public sealed class CreateSouscriptionValidator : BaseEntityValidator<CreateSouscriptionCommand>
{
    public CreateSouscriptionValidator()
    {
        RuleForRequiredFk(x => x.ServiceParentId, "service parent");

        RuleForRequiredFk(x => x.ParentId, "parent");

        RuleFor(x => x.DateDebut)
            .NotEmpty().WithMessage("La date de debut est obligatoire");

        RuleFor(x => x.DateFin)
            .GreaterThan(x => x.DateDebut).WithMessage("La date de fin doit etre posterieure a la date de debut")
            .When(x => x.DateFin.HasValue);

        RuleForEnum(x => x.StatutSouscription, ["Active", "Expiree", "Resiliee", "Essai"], "statut souscription");

        RuleForMoney(x => x.MontantPaye, "montant paye");
    }
}
