using FluentValidation;
using GnValidation.FluentValidation;
using ServicesPremium.Application.Commands;

namespace ServicesPremium.Application.Validators;

/// <summary>
/// Validateur pour la mise a jour d'un service parent.
/// </summary>
public sealed class UpdateServiceParentValidator : BaseEntityValidator<UpdateServiceParentCommand>
{
    public UpdateServiceParentValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire.");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForRequiredString(x => x.Code, 50, "Code");
        RuleForMoney(x => x.Tarif, "Tarif");
        RuleForEnum(x => x.Periodicite, ["Mensuel", "Annuel", "Unite"], "Periodicite");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
