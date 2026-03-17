using GnValidation.Commands.Presences;
using FluentValidation;

namespace GnValidation.FluentValidation.Presences;

/// <summary>
/// Validateur pour la creation d'une absence.
/// Verifie la reference eleve, l'appel optionnel, la date d'absence,
/// le motif de justification et l'URL de piece jointe.
/// </summary>
public sealed class CreateAbsenceValidator : BaseEntityValidator<CreateAbsenceCommand>
{
    public CreateAbsenceValidator()
    {
        RuleForRequiredFk(x => x.EleveId, "eleve");

        RuleFor(x => x.AppelId)
            .GreaterThan(0).WithMessage("L'identifiant de l'appel doit etre superieur a 0")
            .When(x => x.AppelId.HasValue);

        RuleFor(x => x.DateAbsence)
            .NotEmpty().WithMessage("La date d'absence est obligatoire");

        RuleForOptionalString(x => x.MotifJustification, 500, "motif de justification");

        RuleForUrl(x => x.PieceJointeUrl, "piece jointe");
    }
}
