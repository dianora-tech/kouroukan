using GnValidation.Commands.Documents;
using FluentValidation;

namespace GnValidation.FluentValidation.Documents;

/// <summary>
/// Validateur pour la creation d'un document genere.
/// Verifie le nom, le modele de document, les references eleve et enseignant,
/// les donnees JSON et le statut de signature.
/// </summary>
public sealed class CreateDocumentGenereValidator : BaseEntityValidator<CreateDocumentGenereCommand>
{
    public CreateDocumentGenereValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredFk(x => x.ModeleDocumentId, "modele de document");

        RuleFor(x => x.EleveId)
            .GreaterThan(0).WithMessage("L'identifiant de l'eleve doit etre superieur a 0")
            .When(x => x.EleveId.HasValue);

        RuleFor(x => x.EnseignantId)
            .GreaterThan(0).WithMessage("L'identifiant de l'enseignant doit etre superieur a 0")
            .When(x => x.EnseignantId.HasValue);

        RuleFor(x => x.DonneesJson)
            .NotEmpty().WithMessage("Les donnees JSON sont obligatoires");

        RuleForEnum(x => x.StatutSignature, ["EnAttente", "EnCours", "Signe", "Refuse"], "statut signature");
    }
}
