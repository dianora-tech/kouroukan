using GnValidation.Commands.Bde;
using FluentValidation;

namespace GnValidation.FluentValidation.Bde;

/// <summary>
/// Validateur pour la creation d'une depense BDE.
/// Verifie le nom, l'association, le montant, le motif,
/// la categorie et le statut de validation.
/// </summary>
public sealed class CreateDepenseBDEValidator : BaseEntityValidator<CreateDepenseBDECommand>
{
    public CreateDepenseBDEValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredFk(x => x.AssociationId, "association");

        RuleFor(x => x.Montant)
            .GreaterThan(0).WithMessage("Le montant de la depense doit etre superieur a 0");

        RuleForRequiredString(x => x.Motif, 500, "motif");

        RuleForEnum(x => x.Categorie, ["Materiel", "Location", "Prestataire", "Remboursement"], "categorie");

        RuleForEnum(x => x.StatutValidation, ["Demandee", "ValideTresorier", "ValideSuper", "Refusee"], "statut validation");
    }
}
