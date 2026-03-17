using FluentValidation;
using GnValidation.FluentValidation;
using Documents.Application.Commands;

namespace Documents.Application.Validators;

public sealed class UpdateSignatureValidator : BaseEntityValidator<UpdateSignatureCommand>
{
    public UpdateSignatureValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.TypeId, "Type");
        RuleForRequiredFk(x => x.DocumentGenereId, "Document genere");
        RuleForRequiredFk(x => x.SignataireId, "Signataire");
        RuleFor(x => x.OrdreSignature).GreaterThan(0).WithMessage("L'ordre de signature doit etre superieur a 0");
        RuleForEnum(x => x.StatutSignature, ["EnAttente", "Signe", "Refuse", "Delegue"], "Statut signature");
        RuleForEnum(x => x.NiveauSignature, ["Basique", "Avancee"], "Niveau signature");
        RuleForOptionalString(x => x.MotifRefus, 500, "Motif refus");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
