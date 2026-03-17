using Finances.Application.Commands;
using FluentValidation;
using GnValidation.FluentValidation;

namespace Finances.Application.Validators;

public sealed class CreatePaiementValidator : BaseEntityValidator<CreatePaiementCommand>
{
    public CreatePaiementValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type de paiement");
        RuleForRequiredFk(x => x.FactureId, "Facture");
        RuleFor(x => x.MontantPaye).GreaterThan(0).WithMessage("Le montant paye doit etre superieur a zero");
        RuleFor(x => x.DatePaiement).NotEmpty().WithMessage("La date de paiement est obligatoire");
        RuleForEnum(x => x.MoyenPaiement,
            ["OrangeMoney", "SoutraMoney", "MTNMoMo", "Especes"], "Moyen de paiement");
        RuleForOptionalString(x => x.ReferenceMobileMoney, 100, "Reference Mobile Money");
        RuleForEnum(x => x.StatutPaiement,
            ["EnAttente", "Confirme", "Echec", "Rembourse"], "Statut du paiement");
        RuleForRequiredString(x => x.NumeroRecu, 50, "Numero de recu");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
