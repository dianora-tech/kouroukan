using FluentValidation;
using GnValidation.FluentValidation;
using Presences.Application.Commands;

namespace Presences.Application.Validators;

public sealed class UpdateAppelValidator : BaseEntityValidator<UpdateAppelCommand>
{
    public UpdateAppelValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.ClasseId, "Classe");
        RuleForRequiredFk(x => x.EnseignantId, "Enseignant");
        RuleFor(x => x.DateAppel).NotEmpty().WithMessage("La date de l'appel est obligatoire");
        RuleFor(x => x.HeureAppel).NotEqual(TimeSpan.Zero).WithMessage("L'heure de l'appel est obligatoire");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
