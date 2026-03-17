using FluentValidation;
using GnValidation.FluentValidation;
using Pedagogie.Application.Commands;

namespace Pedagogie.Application.Validators;

public sealed class UpdateMatiereValidator : BaseEntityValidator<UpdateMatiereCommand>
{
    public UpdateMatiereValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredString(x => x.Name, 100, "Nom");
        RuleForRequiredString(x => x.Code, 20, "Code");
        RuleFor(x => x.Coefficient).GreaterThan(0).WithMessage("Le coefficient doit etre superieur a 0");
        RuleFor(x => x.NombreHeures).GreaterThan(0).WithMessage("Le nombre d'heures doit etre superieur a 0");
        RuleForRequiredFk(x => x.NiveauClasseId, "Niveau de classe");
    }
}
