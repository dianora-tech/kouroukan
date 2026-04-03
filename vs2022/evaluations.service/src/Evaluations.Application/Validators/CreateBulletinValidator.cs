using FluentValidation;
using GnValidation.FluentValidation;
using Evaluations.Application.Commands;

namespace Evaluations.Application.Validators;

public sealed class CreateBulletinValidator : BaseEntityValidator<CreateBulletinCommand>
{
    public CreateBulletinValidator()
    {
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleForRequiredFk(x => x.ClasseId, "Classe");
        RuleFor(x => x.Trimestre).InclusiveBetween(1, 3).WithMessage("Le trimestre doit etre compris entre 1 et 3");
        RuleForRequiredFk(x => x.AnneeScolaireId, "Annee scolaire");
        RuleFor(x => x.MoyenneGenerale).InclusiveBetween(0, 20).WithMessage("La moyenne generale doit etre comprise entre 0 et 20");
        RuleForOptionalString(x => x.Appreciation, 500, "Appreciation");
        RuleFor(x => x.DateGeneration).NotEmpty().WithMessage("La date de generation est obligatoire");
        RuleForUrl(x => x.CheminFichierPdf, "Chemin fichier PDF");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
