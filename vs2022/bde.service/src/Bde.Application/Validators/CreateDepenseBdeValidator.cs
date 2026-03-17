using FluentValidation;
using GnValidation.FluentValidation;
using Bde.Application.Commands;

namespace Bde.Application.Validators;

public sealed class CreateDepenseBdeValidator : BaseEntityValidator<CreateDepenseBdeCommand>
{
    public CreateDepenseBdeValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredString(x => x.Name, 200, "Libelle");
        RuleForOptionalString(x => x.Description, 1000, "Description");
        RuleForRequiredFk(x => x.AssociationId, "Association");
        RuleForMoney(x => x.Montant, "Montant");
        RuleFor(x => x.Montant).GreaterThan(0).WithMessage("Le montant doit etre superieur a zero");
        RuleForRequiredString(x => x.Motif, 500, "Motif");
        RuleForEnum(x => x.Categorie, ["Materiel", "Location", "Prestataire", "Remboursement"], "Categorie");
        RuleForEnum(x => x.StatutValidation, ["Demandee", "ValideTresorier", "ValideSuper", "Refusee"], "Statut validation");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
