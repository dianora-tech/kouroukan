using FluentValidation;
using GnValidation.FluentValidation;
using Pedagogie.Application.Commands;

namespace Pedagogie.Application.Validators;

public sealed class CreateCahierTextesValidator : BaseEntityValidator<CreateCahierTextesCommand>
{
    public CreateCahierTextesValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "Titre");
        RuleForRequiredFk(x => x.SeanceId, "Seance");
        RuleFor(x => x.Contenu).NotEmpty().WithMessage("Le contenu est obligatoire");
        RuleFor(x => x.DateSeance).NotEmpty().WithMessage("La date de la seance est obligatoire");
    }
}
