using GnValidation.Commands.Evaluations;
using FluentValidation;

namespace GnValidation.FluentValidation.Evaluations;

/// <summary>
/// Validateur pour la creation d'une evaluation.
/// Verifie le nom, les references matiere, classe, enseignant et annee scolaire,
/// ainsi que la date, le coefficient, la note maximale et le trimestre.
/// </summary>
public sealed class CreateEvaluationValidator : BaseEntityValidator<CreateEvaluationCommand>
{
    public CreateEvaluationValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredFk(x => x.MatiereId, "matiere");

        RuleForRequiredFk(x => x.ClasseId, "classe");

        RuleForRequiredFk(x => x.EnseignantId, "enseignant");

        RuleFor(x => x.DateEvaluation)
            .NotEmpty().WithMessage("La date d'evaluation est obligatoire");

        RuleFor(x => x.Coefficient)
            .GreaterThan(0).WithMessage("Le coefficient doit etre superieur a 0");

        RuleFor(x => x.NoteMaximale)
            .GreaterThan(0).WithMessage("La note maximale doit etre superieure a 0");

        RuleFor(x => x.Trimestre)
            .InclusiveBetween(1, 3).WithMessage("Le trimestre doit etre entre 1 et 3");

        RuleForRequiredFk(x => x.AnneeScolaireId, "annee scolaire");
    }
}
