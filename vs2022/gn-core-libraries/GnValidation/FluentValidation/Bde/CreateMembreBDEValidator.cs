using GnValidation.Commands.Bde;
using FluentValidation;

namespace GnValidation.FluentValidation.Bde;

/// <summary>
/// Validateur pour la creation d'un membre BDE.
/// Verifie l'association, l'eleve, le role BDE, la date d'adhesion
/// et le montant de cotisation optionnel.
/// </summary>
public sealed class CreateMembreBDEValidator : BaseEntityValidator<CreateMembreBDECommand>
{
    public CreateMembreBDEValidator()
    {
        RuleForRequiredFk(x => x.AssociationId, "association");

        RuleForRequiredFk(x => x.EleveId, "eleve");

        RuleForEnum(x => x.RoleBDE, ["President", "Tresorier", "Secretaire", "RespPole", "Membre"], "role BDE");

        RuleFor(x => x.DateAdhesion)
            .NotEmpty().WithMessage("La date d'adhesion est obligatoire");

        RuleFor(x => x.MontantCotisation)
            .GreaterThanOrEqualTo(0).WithMessage("Le montant de cotisation ne peut pas etre negatif")
            .When(x => x.MontantCotisation.HasValue);
    }
}
