using FluentValidation;
using GnValidation.FluentValidation;
using Evaluations.Application.Commands;

namespace Evaluations.Application.Validators;

public sealed class UpdateNoteValidator : BaseEntityValidator<UpdateNoteCommand>
{
    public UpdateNoteValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.EvaluationId, "Evaluation");
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleFor(x => x.Valeur).GreaterThanOrEqualTo(0).WithMessage("La note ne peut pas etre negative");
        RuleForOptionalString(x => x.Commentaire, 500, "Commentaire");
        RuleFor(x => x.DateSaisie).NotEmpty().WithMessage("La date de saisie est obligatoire");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
