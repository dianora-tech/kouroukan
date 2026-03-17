using FluentValidation;
using GnValidation.FluentValidation;
using Pedagogie.Application.Commands;

namespace Pedagogie.Application.Validators;

public sealed class CreateNiveauClasseValidator : BaseEntityValidator<CreateNiveauClasseCommand>
{
    public CreateNiveauClasseValidator()
    {
        RuleForRequiredString(x => x.Name, 100, "Nom");
        RuleForRequiredString(x => x.Code, 20, "Code");
        RuleFor(x => x.Ordre).GreaterThan(0).WithMessage("L'ordre doit etre superieur a 0");
        RuleForRequiredString(x => x.CycleEtude, 50, "Cycle d'etude");
        RuleForEnum(x => x.CycleEtude,
            new[] { "Prescolaire", "Primaire", "College", "Lycee", "ETFP_PostPrimaire", "ETFP_TypeA", "ETFP_TypeB", "ENF", "Universite" },
            "Cycle d'etude");
        RuleForOptionalEnum(x => x.MinistereTutelle,
            new[] { "MENA", "METFP-ET", "MESRS" },
            "Ministere de tutelle");
    }
}
