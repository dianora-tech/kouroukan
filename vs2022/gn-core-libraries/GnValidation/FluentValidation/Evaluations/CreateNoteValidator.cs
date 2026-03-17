using GnValidation.Commands.Evaluations;
using FluentValidation;

namespace GnValidation.FluentValidation.Evaluations;

/// <summary>
/// Validateur pour la creation d'une note.
/// Verifie les references evaluation et eleve, la valeur de la note et le commentaire optionnel.
/// </summary>
public sealed class CreateNoteValidator : BaseEntityValidator<CreateNoteCommand>
{
    public CreateNoteValidator()
    {
        RuleForRequiredFk(x => x.EvaluationId, "evaluation");

        RuleForRequiredFk(x => x.EleveId, "eleve");

        RuleFor(x => x.Valeur)
            .GreaterThanOrEqualTo(0).WithMessage("La note ne peut pas etre negative");

        RuleForOptionalString(x => x.Commentaire, 500, "commentaire");
    }
}
