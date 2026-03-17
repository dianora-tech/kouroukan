using FluentValidation;
using GnValidation.FluentValidation;
using Pedagogie.Application.Commands;

namespace Pedagogie.Application.Validators;

public sealed class CreateClasseValidator : BaseEntityValidator<CreateClasseCommand>
{
    public CreateClasseValidator()
    {
        RuleForRequiredString(x => x.Name, 100, "Nom");
        RuleFor(x => x.Capacite).GreaterThan(0).WithMessage("La capacite doit etre superieure a 0");
        RuleForRequiredFk(x => x.NiveauClasseId, "Niveau de classe");
        RuleForRequiredFk(x => x.AnneeScolaireId, "Annee scolaire");
        RuleFor(x => x.Effectif).GreaterThanOrEqualTo(0).WithMessage("L'effectif ne peut pas etre negatif");
    }
}
