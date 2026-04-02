using FluentValidation;
using GnValidation.FluentValidation;
using Pedagogie.Application.Commands;

namespace Pedagogie.Application.Validators;

public sealed class CreateMatiereValidator : BaseEntityValidator<CreateMatiereCommand>
{
    public CreateMatiereValidator()
    {
        RuleForRequiredString(x => x.Name, 100, "Nom");
        RuleForRequiredString(x => x.Code, 20, "Code");
    }
}
