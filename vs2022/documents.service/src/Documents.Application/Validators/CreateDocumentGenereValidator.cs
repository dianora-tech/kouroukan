using FluentValidation;
using GnValidation.FluentValidation;
using Documents.Application.Commands;

namespace Documents.Application.Validators;

public sealed class CreateDocumentGenereValidator : BaseEntityValidator<CreateDocumentGenereCommand>
{
    public CreateDocumentGenereValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.ModeleDocumentId, "Modele de document");
        RuleFor(x => x.DonneesJson).NotEmpty().WithMessage("Les donnees JSON de merge sont obligatoires");
        RuleFor(x => x.DateGeneration).NotEmpty().WithMessage("La date de generation est obligatoire");
        RuleForEnum(x => x.StatutSignature, ["EnAttente", "EnCours", "Signe", "Refuse"], "Statut signature");
        RuleForUrl(x => x.CheminFichier, "Chemin fichier");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
