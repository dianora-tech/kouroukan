using FluentValidation;
using GnValidation.FluentValidation;
using Evaluations.Application.Commands;

namespace Evaluations.Application.Validators;

public sealed class UpdateEvaluationValidator : BaseEntityValidator<UpdateEvaluationCommand>
{
    public UpdateEvaluationValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.MatiereId, "Matiere");
        RuleForRequiredFk(x => x.ClasseId, "Classe");
        RuleForRequiredFk(x => x.EnseignantId, "Enseignant");
        RuleFor(x => x.DateEvaluation).NotEmpty().WithMessage("La date d'evaluation est obligatoire");
        RuleForMoney(x => x.Coefficient, "Coefficient");
        RuleForMoney(x => x.NoteMaximale, "Note maximale");
        RuleFor(x => x.Trimestre).InclusiveBetween(1, 3).WithMessage("Le trimestre doit etre compris entre 1 et 3");
        RuleForRequiredFk(x => x.AnneeScolaireId, "Annee scolaire");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
