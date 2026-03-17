using FluentValidation;
using GnValidation.FluentValidation;
using ServicesPremium.Application.Commands;

namespace ServicesPremium.Application.Validators;

/// <summary>
/// Validateur pour la creation d'une souscription.
/// </summary>
public sealed class CreateSouscriptionValidator : BaseEntityValidator<CreateSouscriptionCommand>
{
    public CreateSouscriptionValidator()
    {
        RuleForRequiredFk(x => x.ServiceParentId, "Service parent");
        RuleForRequiredFk(x => x.ParentId, "Parent");
        RuleFor(x => x.DateDebut).NotEmpty().WithMessage("La date de debut est obligatoire.");
        RuleForEnum(x => x.StatutSouscription, ["Active", "Expiree", "Resiliee", "Essai"], "Statut souscription");
        RuleForMoney(x => x.MontantPaye, "Montant paye");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
