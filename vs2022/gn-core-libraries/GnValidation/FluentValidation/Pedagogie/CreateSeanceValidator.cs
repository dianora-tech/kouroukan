using GnValidation.Commands.Pedagogie;
using FluentValidation;

namespace GnValidation.FluentValidation.Pedagogie;

/// <summary>
/// Validateur pour la creation d'une seance.
/// Verifie la matiere, la classe, l'enseignant, la salle, le jour, les horaires et l'annee scolaire.
/// </summary>
public sealed class CreateSeanceValidator : BaseEntityValidator<CreateSeanceCommand>
{
    public CreateSeanceValidator()
    {
        RuleForRequiredFk(x => x.MatiereId, "matiere");

        RuleForRequiredFk(x => x.ClasseId, "classe");

        RuleForRequiredFk(x => x.EnseignantId, "enseignant");

        RuleForRequiredFk(x => x.SalleId, "salle");

        RuleFor(x => x.JourSemaine)
            .InclusiveBetween(1, 6).WithMessage("Le jour de la semaine doit etre entre 1 (lundi) et 6 (samedi)");

        RuleFor(x => x.HeureDebut)
            .NotEqual(default(TimeSpan)).WithMessage("L'heure de debut est obligatoire");

        RuleFor(x => x.HeureFin)
            .NotEqual(default(TimeSpan)).WithMessage("L'heure de fin est obligatoire")
            .GreaterThan(x => x.HeureDebut).WithMessage("L'heure de fin doit etre posterieure a l'heure de debut");

        RuleForRequiredFk(x => x.AnneeScolaireId, "annee scolaire");
    }
}
