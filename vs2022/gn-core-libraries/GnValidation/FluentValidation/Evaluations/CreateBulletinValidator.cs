using GnValidation.Commands.Evaluations;
using FluentValidation;

namespace GnValidation.FluentValidation.Evaluations;

/// <summary>
/// Validateur pour la creation d'un bulletin.
/// Verifie les references eleve, classe et annee scolaire, le trimestre,
/// la moyenne generale, le rang optionnel et l'appreciation.
/// </summary>
public sealed class CreateBulletinValidator : BaseEntityValidator<CreateBulletinCommand>
{
    public CreateBulletinValidator()
    {
        RuleForRequiredFk(x => x.EleveId, "eleve");

        RuleForRequiredFk(x => x.ClasseId, "classe");

        RuleFor(x => x.Trimestre)
            .InclusiveBetween(1, 3).WithMessage("Le trimestre doit etre entre 1 et 3");

        RuleForRequiredFk(x => x.AnneeScolaireId, "annee scolaire");

        RuleFor(x => x.MoyenneGenerale)
            .InclusiveBetween(0, 20).WithMessage("La moyenne doit etre entre 0 et 20");

        RuleFor(x => x.Rang)
            .GreaterThan(0).WithMessage("Le rang doit etre superieur a 0")
            .When(x => x.Rang.HasValue);

        RuleForOptionalString(x => x.Appreciation, 500, "appreciation");
    }
}
