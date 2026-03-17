using FluentValidation;
using GnValidation.FluentValidation;
using ServicesPremium.Application.Commands;

namespace ServicesPremium.Application.Validators;

/// <summary>
/// Validateur pour la creation d'un service parent.
/// </summary>
public sealed class CreateServiceParentValidator : BaseEntityValidator<CreateServiceParentCommand>
{
    public CreateServiceParentValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForRequiredString(x => x.Code, 50, "Code");
        RuleForMoney(x => x.Tarif, "Tarif");
        RuleForEnum(x => x.Periodicite, ["Mensuel", "Annuel", "Unite"], "Periodicite");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
