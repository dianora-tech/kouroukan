using GnValidation.Commands.Presences;
using FluentValidation;

namespace GnValidation.FluentValidation.Presences;

/// <summary>
/// Validateur pour la creation d'un appel.
/// Verifie les references classe et enseignant, la seance optionnelle,
/// la date et l'heure d'appel.
/// </summary>
public sealed class CreateAppelValidator : BaseEntityValidator<CreateAppelCommand>
{
    public CreateAppelValidator()
    {
        RuleForRequiredFk(x => x.ClasseId, "classe");

        RuleForRequiredFk(x => x.EnseignantId, "enseignant");

        RuleFor(x => x.SeanceId)
            .GreaterThan(0).WithMessage("L'identifiant de la seance doit etre superieur a 0")
            .When(x => x.SeanceId.HasValue);

        RuleFor(x => x.DateAppel)
            .NotEmpty().WithMessage("La date d'appel est obligatoire");

        RuleFor(x => x.HeureAppel)
            .NotEqual(default(TimeSpan)).WithMessage("L'heure d'appel est obligatoire");
    }
}
