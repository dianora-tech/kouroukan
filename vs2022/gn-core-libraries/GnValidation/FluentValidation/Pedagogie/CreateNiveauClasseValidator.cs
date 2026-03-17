using GnValidation.Commands.Pedagogie;
using FluentValidation;

namespace GnValidation.FluentValidation.Pedagogie;

/// <summary>
/// Validateur pour la creation d'un niveau de classe.
/// Verifie le nom, le code, l'ordre, le cycle d'etude et les informations complementaires.
/// </summary>
public sealed class CreateNiveauClasseValidator : BaseEntityValidator<CreateNiveauClasseCommand>
{
    public CreateNiveauClasseValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredString(x => x.Code, 20, "code");

        RuleFor(x => x.Ordre)
            .GreaterThan(0).WithMessage("L'ordre doit etre superieur a 0");

        RuleForEnum(x => x.CycleEtude, ["Prescolaire", "Primaire", "College", "Lycee", "ETFP_PostPrimaire", "ETFP_TypeA", "ETFP_TypeB", "ENF", "Universite"], "cycle d'etude");

        RuleFor(x => x.AgeOfficielEntree)
            .InclusiveBetween(3, 30).WithMessage("L'age officiel d'entree doit etre entre 3 et 30 ans")
            .When(x => x.AgeOfficielEntree.HasValue);

        RuleForOptionalEnum(x => x.MinistereTutelle, ["MENA", "METFP-ET", "MESRS"], "ministere de tutelle");

        RuleForOptionalEnum(x => x.ExamenSortie, ["CEE", "BEPC", "BU", "CQP", "BEP", "CAP", "BTS", "Licence", "Master", "Doctorat"], "examen de sortie");

        RuleFor(x => x.TauxHoraireEnseignant)
            .GreaterThanOrEqualTo(0).WithMessage("Le taux horaire ne peut pas etre negatif")
            .When(x => x.TauxHoraireEnseignant.HasValue);
    }
}
