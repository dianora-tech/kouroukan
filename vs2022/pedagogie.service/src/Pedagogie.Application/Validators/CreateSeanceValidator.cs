using FluentValidation;
using GnValidation.FluentValidation;
using Pedagogie.Application.Commands;

namespace Pedagogie.Application.Validators;

public sealed class CreateSeanceValidator : BaseEntityValidator<CreateSeanceCommand>
{
    public CreateSeanceValidator()
    {
        RuleForRequiredString(x => x.Name, 100, "Nom");
        RuleForRequiredFk(x => x.MatiereId, "Matiere");
        RuleForRequiredFk(x => x.ClasseId, "Classe");
        RuleForRequiredFk(x => x.EnseignantId, "Enseignant");
        RuleForRequiredFk(x => x.SalleId, "Salle");
        RuleFor(x => x.JourSemaine).InclusiveBetween(1, 6).WithMessage("Le jour doit etre entre 1 (Lundi) et 6 (Samedi)");
        RuleFor(x => x.HeureFin).GreaterThan(x => x.HeureDebut).WithMessage("L'heure de fin doit etre posterieure a l'heure de debut");
        RuleForRequiredFk(x => x.AnneeScolaireId, "Annee scolaire");
    }
}
