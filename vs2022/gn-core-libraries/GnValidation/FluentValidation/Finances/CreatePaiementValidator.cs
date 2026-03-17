using GnValidation.Commands.Finances;
using FluentValidation;

namespace GnValidation.FluentValidation.Finances;

/// <summary>
/// Validateur pour la creation d'un paiement.
/// Verifie la reference facture, le montant paye, la date de paiement,
/// le moyen et le statut de paiement, la reference Mobile Money,
/// le caissier optionnel et le numero de recu.
/// </summary>
public sealed class CreatePaiementValidator : BaseEntityValidator<CreatePaiementCommand>
{
    public CreatePaiementValidator()
    {
        RuleForRequiredFk(x => x.FactureId, "facture");

        RuleFor(x => x.MontantPaye)
            .GreaterThan(0).WithMessage("Le montant paye doit etre superieur a 0");

        RuleFor(x => x.DatePaiement)
            .NotEmpty().WithMessage("La date de paiement est obligatoire");

        RuleForEnum(x => x.MoyenPaiement, ["OrangeMoney", "SoutraMoney", "MTNMoMo", "Especes"], "moyen de paiement");

        RuleForOptionalString(x => x.ReferenceMobileMoney, 100, "reference Mobile Money");

        RuleForEnum(x => x.StatutPaiement, ["EnAttente", "Confirme", "Echec", "Rembourse"], "statut paiement");

        RuleFor(x => x.CaissierId)
            .GreaterThan(0).WithMessage("L'identifiant du caissier doit etre superieur a 0")
            .When(x => x.CaissierId.HasValue);

        RuleForRequiredString(x => x.NumeroRecu, 50, "numero de recu");
    }
}
