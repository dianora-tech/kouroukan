using FluentValidation;
using GnValidation.FluentValidation;
using Presences.Application.Commands;

namespace Presences.Application.Validators;

public sealed class CreateAbsenceValidator : BaseEntityValidator<CreateAbsenceCommand>
{
    public CreateAbsenceValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleFor(x => x.DateAbsence).NotEmpty().WithMessage("La date de l'absence est obligatoire");
        RuleForOptionalString(x => x.MotifJustification, 500, "Motif justification");
        RuleForUrl(x => x.PieceJointeUrl, "Piece jointe");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
