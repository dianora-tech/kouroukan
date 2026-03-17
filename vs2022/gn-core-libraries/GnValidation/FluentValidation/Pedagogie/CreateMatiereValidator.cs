using GnValidation.Commands.Pedagogie;
using FluentValidation;

namespace GnValidation.FluentValidation.Pedagogie;

/// <summary>
/// Validateur pour la creation d'une matiere.
/// Verifie le nom, le code, le coefficient, le nombre d'heures et le niveau de classe.
/// </summary>
public sealed class CreateMatiereValidator : BaseEntityValidator<CreateMatiereCommand>
{
    public CreateMatiereValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredString(x => x.Code, 20, "code");

        RuleFor(x => x.Coefficient)
            .GreaterThan(0).WithMessage("Le coefficient doit etre superieur a 0");

        RuleFor(x => x.NombreHeures)
            .GreaterThan(0).WithMessage("Le nombre d'heures doit etre superieur a 0");

        RuleForRequiredFk(x => x.NiveauClasseId, "niveau de classe");
    }
}
