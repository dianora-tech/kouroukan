using GnValidation.Commands.Bde;
using FluentValidation;

namespace GnValidation.FluentValidation.Bde;

/// <summary>
/// Validateur pour la creation d'une association.
/// Verifie le nom, le sigle optionnel, l'annee scolaire, le statut,
/// le budget annuel et le superviseur optionnel.
/// </summary>
public sealed class CreateAssociationValidator : BaseEntityValidator<CreateAssociationCommand>
{
    public CreateAssociationValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForOptionalString(x => x.Sigle, 50, "sigle");

        RuleForRequiredString(x => x.AnneeScolaire, 20, "annee scolaire");

        RuleForEnum(x => x.Statut, ["Active", "Suspendue", "Dissoute"], "statut");

        RuleForMoney(x => x.BudgetAnnuel, "budget annuel");

        RuleFor(x => x.SuperviseurId)
            .GreaterThan(0).WithMessage("L'identifiant du superviseur doit etre superieur a 0")
            .When(x => x.SuperviseurId.HasValue);
    }
}
