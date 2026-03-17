using GnValidation.Commands.Pedagogie;
using FluentValidation;

namespace GnValidation.FluentValidation.Pedagogie;

/// <summary>
/// Validateur pour la creation d'une salle.
/// Verifie le nom, la capacite et le batiment.
/// </summary>
public sealed class CreateSalleValidator : BaseEntityValidator<CreateSalleCommand>
{
    public CreateSalleValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleFor(x => x.Capacite)
            .GreaterThan(0).WithMessage("La capacite doit etre superieure a 0");

        RuleForOptionalString(x => x.Batiment, 100, "batiment");
    }
}
