using GnValidation.Commands.Bde;
using FluentValidation;

namespace GnValidation.FluentValidation.Bde;

/// <summary>
/// Validateur pour la creation d'un evenement.
/// Verifie le nom, l'association, la date, le lieu, la capacite optionnelle,
/// le tarif d'entree optionnel et le statut de l'evenement.
/// </summary>
public sealed class CreateEvenementValidator : BaseEntityValidator<CreateEvenementCommand>
{
    public CreateEvenementValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredFk(x => x.AssociationId, "association");

        RuleFor(x => x.DateEvenement)
            .NotEmpty().WithMessage("La date de l'evenement est obligatoire");

        RuleForRequiredString(x => x.Lieu, 200, "lieu");

        RuleFor(x => x.Capacite)
            .GreaterThan(0).WithMessage("La capacite doit etre superieure a 0")
            .When(x => x.Capacite.HasValue);

        RuleFor(x => x.TarifEntree)
            .GreaterThanOrEqualTo(0).WithMessage("Le tarif d'entree ne peut pas etre negatif")
            .When(x => x.TarifEntree.HasValue);

        RuleForEnum(x => x.StatutEvenement, ["Planifie", "Valide", "EnCours", "Termine", "Annule"], "statut evenement");
    }
}
