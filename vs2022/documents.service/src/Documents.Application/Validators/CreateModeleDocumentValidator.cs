using FluentValidation;
using GnValidation.FluentValidation;
using Documents.Application.Commands;

namespace Documents.Application.Validators;

public sealed class CreateModeleDocumentValidator : BaseEntityValidator<CreateModeleDocumentCommand>
{
    public CreateModeleDocumentValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredString(x => x.Code, 50, "Code");
        RuleFor(x => x.Contenu).NotEmpty().WithMessage("Le contenu du modele est obligatoire");
        RuleForUrl(x => x.LogoUrl, "Logo URL");
        RuleForOptionalString(x => x.CouleurPrimaire, 7, "Couleur primaire");
        RuleForOptionalString(x => x.TextePiedPage, 500, "Texte pied de page");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
