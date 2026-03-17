using FluentValidation;
using GnValidation.FluentValidation;
using Bde.Application.Commands;

namespace Bde.Application.Validators;

public sealed class UpdateAssociationValidator : BaseEntityValidator<UpdateAssociationCommand>
{
    public UpdateAssociationValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForOptionalString(x => x.Description, 1000, "Description");
        RuleForOptionalString(x => x.Sigle, 50, "Sigle");
        RuleForRequiredString(x => x.AnneeScolaire, 20, "Annee scolaire");
        RuleForEnum(x => x.Statut, ["Active", "Suspendue", "Dissoute"], "Statut");
        RuleForMoney(x => x.BudgetAnnuel, "Budget annuel");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
