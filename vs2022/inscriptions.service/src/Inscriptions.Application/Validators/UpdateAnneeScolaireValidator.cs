using FluentValidation;
using GnValidation.FluentValidation;
using Inscriptions.Application.Commands;

namespace Inscriptions.Application.Validators;

public sealed class UpdateAnneeScolaireValidator : BaseEntityValidator<UpdateAnneeScolaireCommand>
{
    private static readonly string[] StatutsValides = { "preparation", "active", "cloturee", "archivee" };
    private static readonly string[] TypesPeriodesValides = { "trimestre", "semestre" };

    public UpdateAnneeScolaireValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredString(x => x.Libelle, 20, "Libelle");
        RuleFor(x => x.DateDebut).NotEmpty().WithMessage("La date de debut est obligatoire");
        RuleFor(x => x.DateFin)
            .NotEmpty().WithMessage("La date de fin est obligatoire")
            .GreaterThan(x => x.DateDebut).WithMessage("La date de fin doit etre posterieure a la date de debut");

        RuleFor(x => x.Code)
            .MaximumLength(20).WithMessage("Le code ne doit pas depasser 20 caracteres")
            .When(x => x.Code is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La description ne doit pas depasser 500 caracteres")
            .When(x => x.Description is not null);

        RuleFor(x => x.Statut)
            .Must(s => StatutsValides.Contains(s))
            .WithMessage("Le statut doit etre l'une des valeurs suivantes : preparation, active, cloturee, archivee");

        RuleFor(x => x.NombrePeriodes)
            .GreaterThanOrEqualTo(1).WithMessage("Le nombre de periodes doit etre au moins 1");

        RuleFor(x => x.TypePeriode)
            .Must(t => TypesPeriodesValides.Contains(t))
            .WithMessage("Le type de periode doit etre trimestre ou semestre");
    }
}
