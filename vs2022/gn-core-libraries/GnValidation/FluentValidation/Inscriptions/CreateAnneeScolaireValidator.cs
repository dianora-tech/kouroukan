using GnValidation.Commands.Inscriptions;
using FluentValidation;

namespace GnValidation.FluentValidation.Inscriptions;

/// <summary>
/// Validateur pour la creation d'une annee scolaire.
/// Verifie le libelle et la coherence des dates de debut et de fin.
/// </summary>
public sealed class CreateAnneeScolaireValidator : BaseEntityValidator<CreateAnneeScolaireCommand>
{
    public CreateAnneeScolaireValidator()
    {
        RuleForRequiredString(x => x.Libelle, 20, "libelle");

        RuleFor(x => x.DateDebut)
            .NotEmpty().WithMessage("La date de debut est obligatoire");

        RuleFor(x => x.DateFin)
            .NotEmpty().WithMessage("La date de fin est obligatoire")
            .GreaterThan(x => x.DateDebut).WithMessage("La date de fin doit etre posterieure a la date de debut");
    }
}
