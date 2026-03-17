using FluentValidation;
using GnValidation.FluentValidation;
using Pedagogie.Application.Commands;

namespace Pedagogie.Application.Validators;

public sealed class UpdateCahierTextesValidator : BaseEntityValidator<UpdateCahierTextesCommand>
{
    public UpdateCahierTextesValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredString(x => x.Name, 200, "Titre");
        RuleForRequiredFk(x => x.SeanceId, "Seance");
        RuleFor(x => x.Contenu).NotEmpty().WithMessage("Le contenu est obligatoire");
        RuleFor(x => x.DateSeance).NotEmpty().WithMessage("La date de la seance est obligatoire");
    }
}
