using FluentValidation;
using GnValidation.FluentValidation;
using Presences.Application.Commands;

namespace Presences.Application.Validators;

public sealed class CreateAppelValidator : BaseEntityValidator<CreateAppelCommand>
{
    public CreateAppelValidator()
    {
        RuleForRequiredFk(x => x.ClasseId, "Classe");
        RuleForRequiredFk(x => x.EnseignantId, "Enseignant");
        RuleFor(x => x.DateAppel).NotEmpty().WithMessage("La date de l'appel est obligatoire");
        RuleFor(x => x.HeureAppel).NotEqual(TimeSpan.Zero).WithMessage("L'heure de l'appel est obligatoire");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
