using GnValidation.Commands.Pedagogie;
using FluentValidation;

namespace GnValidation.FluentValidation.Pedagogie;

/// <summary>
/// Validateur pour la creation d'un cahier de textes.
/// Verifie la seance, le contenu et la date de seance.
/// </summary>
public sealed class CreateCahierTextesValidator : BaseEntityValidator<CreateCahierTextesCommand>
{
    public CreateCahierTextesValidator()
    {
        RuleForRequiredFk(x => x.SeanceId, "seance");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu est obligatoire");

        RuleFor(x => x.DateSeance)
            .NotEmpty().WithMessage("La date de seance est obligatoire");
    }
}
