using GnValidation.Commands.Pedagogie;
using FluentValidation;

namespace GnValidation.FluentValidation.Pedagogie;

/// <summary>
/// Validateur pour la creation d'une classe.
/// Verifie le nom, le niveau de classe, la capacite, l'annee scolaire et l'enseignant principal.
/// </summary>
public sealed class CreateClasseValidator : BaseEntityValidator<CreateClasseCommand>
{
    public CreateClasseValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredFk(x => x.NiveauClasseId, "niveau de classe");

        RuleFor(x => x.Capacite)
            .GreaterThan(0).WithMessage("La capacite doit etre superieure a 0");

        RuleForRequiredFk(x => x.AnneeScolaireId, "annee scolaire");

        RuleFor(x => x.EnseignantPrincipalId)
            .GreaterThan(0).WithMessage("L'identifiant de l'enseignant principal doit etre superieur a 0")
            .When(x => x.EnseignantPrincipalId.HasValue);
    }
}
