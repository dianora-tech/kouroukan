using GnValidation.Commands.Finances;
using FluentValidation;

namespace GnValidation.FluentValidation.Finances;

/// <summary>
/// Validateur pour la creation d'une remuneration enseignant.
/// Verifie la reference enseignant, le mois, l'annee, le mode de remuneration,
/// les montants optionnels (forfait, heures, taux horaire),
/// le montant total et le statut de paiement.
/// </summary>
public sealed class CreateRemunerationEnseignantValidator : BaseEntityValidator<CreateRemunerationEnseignantCommand>
{
    public CreateRemunerationEnseignantValidator()
    {
        RuleForRequiredFk(x => x.EnseignantId, "enseignant");

        RuleFor(x => x.Mois)
            .InclusiveBetween(1, 12).WithMessage("Le mois doit etre entre 1 et 12");

        RuleFor(x => x.Annee)
            .InclusiveBetween(2000, 2100).WithMessage("L'annee doit etre entre 2000 et 2100");

        RuleForEnum(x => x.ModeRemuneration, ["Forfait", "Heures", "Mixte"], "mode de remuneration");

        RuleFor(x => x.MontantForfait)
            .GreaterThanOrEqualTo(0).WithMessage("Le montant forfait ne peut pas etre negatif")
            .When(x => x.MontantForfait.HasValue);

        RuleFor(x => x.NombreHeures)
            .GreaterThanOrEqualTo(0).WithMessage("Le nombre d'heures ne peut pas etre negatif")
            .When(x => x.NombreHeures.HasValue);

        RuleFor(x => x.TauxHoraire)
            .GreaterThanOrEqualTo(0).WithMessage("Le taux horaire ne peut pas etre negatif")
            .When(x => x.TauxHoraire.HasValue);

        RuleForMoney(x => x.MontantTotal, "montant total");

        RuleForEnum(x => x.StatutPaiement, ["EnAttente", "Valide", "Paye"], "statut paiement");
    }
}
