using FluentValidation;
using GnValidation.FluentValidation;
using ServicesPremium.Application.Commands;

namespace ServicesPremium.Application.Validators;

/// <summary>
/// Validateur pour la mise a jour d'une souscription.
/// </summary>
public sealed class UpdateSouscriptionValidator : BaseEntityValidator<UpdateSouscriptionCommand>
{
    public UpdateSouscriptionValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire.");
        RuleForRequiredFk(x => x.ServiceParentId, "Service parent");
        RuleForRequiredFk(x => x.ParentId, "Parent");
        RuleFor(x => x.DateDebut).NotEmpty().WithMessage("La date de debut est obligatoire.");
        RuleForEnum(x => x.StatutSouscription, ["Active", "Expiree", "Resiliee", "Essai"], "Statut souscription");
        RuleForMoney(x => x.MontantPaye, "Montant paye");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
