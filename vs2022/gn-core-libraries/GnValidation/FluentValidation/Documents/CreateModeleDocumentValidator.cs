using GnValidation.Commands.Documents;
using FluentValidation;

namespace GnValidation.FluentValidation.Documents;

/// <summary>
/// Validateur pour la creation d'un modele de document.
/// Verifie le nom, le code, le contenu, l'URL du logo,
/// la couleur primaire et le texte de pied de page.
/// </summary>
public sealed class CreateModeleDocumentValidator : BaseEntityValidator<CreateModeleDocumentCommand>
{
    public CreateModeleDocumentValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredString(x => x.Code, 50, "code");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu du modele est obligatoire");

        RuleForUrl(x => x.LogoUrl, "logo");

        RuleFor(x => x.CouleurPrimaire)
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("La couleur primaire doit etre au format hexadecimal (#RRGGBB)")
            .When(x => !string.IsNullOrEmpty(x.CouleurPrimaire));

        RuleForOptionalString(x => x.TextePiedPage, 500, "texte pied de page");
    }
}
