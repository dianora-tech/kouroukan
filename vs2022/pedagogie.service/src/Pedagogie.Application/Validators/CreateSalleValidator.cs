using FluentValidation;
using GnValidation.FluentValidation;
using Pedagogie.Application.Commands;

namespace Pedagogie.Application.Validators;

public sealed class CreateSalleValidator : BaseEntityValidator<CreateSalleCommand>
{
    public CreateSalleValidator()
    {
        RuleForRequiredString(x => x.Name, 100, "Nom");
        RuleFor(x => x.Capacite).GreaterThan(0).WithMessage("La capacite doit etre superieure a 0");
    }
}
