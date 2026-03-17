using FluentValidation;
using GnValidation.FluentValidation;
using Inscriptions.Application.Commands;

namespace Inscriptions.Application.Validators;

public sealed class CreateAnneeScolaireValidator : BaseEntityValidator<CreateAnneeScolaireCommand>
{
    public CreateAnneeScolaireValidator()
    {
        RuleForRequiredString(x => x.Libelle, 20, "Libelle");
        RuleFor(x => x.DateDebut).NotEmpty().WithMessage("La date de debut est obligatoire");
        RuleFor(x => x.DateFin)
            .NotEmpty().WithMessage("La date de fin est obligatoire")
            .GreaterThan(x => x.DateDebut).WithMessage("La date de fin doit etre posterieure a la date de debut");
    }
}
