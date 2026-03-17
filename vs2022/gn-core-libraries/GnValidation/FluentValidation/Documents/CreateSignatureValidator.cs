using GnValidation.Commands.Documents;
using FluentValidation;

namespace GnValidation.FluentValidation.Documents;

/// <summary>
/// Validateur pour la creation d'une signature.
/// Verifie le document genere, le signataire, l'ordre de signature,
/// le statut et le niveau de signature.
/// </summary>
public sealed class CreateSignatureValidator : BaseEntityValidator<CreateSignatureCommand>
{
    public CreateSignatureValidator()
    {
        RuleForRequiredFk(x => x.DocumentGenereId, "document genere");

        RuleForRequiredFk(x => x.SignataireId, "signataire");

        RuleFor(x => x.OrdreSignature)
            .GreaterThan(0).WithMessage("L'ordre de signature doit etre superieur a 0");

        RuleForEnum(x => x.StatutSignature, ["EnAttente", "Signe", "Refuse", "Delegue"], "statut signature");

        RuleForEnum(x => x.NiveauSignature, ["Basique", "Avancee"], "niveau signature");
    }
}
