using GnValidation.Commands.Finances;
using FluentValidation;

namespace GnValidation.FluentValidation.Finances;

/// <summary>
/// Validateur pour la creation d'une facture.
/// Verifie les references eleve, parent et annee scolaire,
/// les montants, les dates d'emission et d'echeance,
/// le statut et le numero de facture.
/// </summary>
public sealed class CreateFactureValidator : BaseEntityValidator<CreateFactureCommand>
{
    public CreateFactureValidator()
    {
        RuleForRequiredFk(x => x.EleveId, "eleve");

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("L'identifiant du parent doit etre superieur a 0")
            .When(x => x.ParentId.HasValue);

        RuleForRequiredFk(x => x.AnneeScolaireId, "annee scolaire");

        RuleForMoney(x => x.MontantTotal, "montant total");

        RuleForMoney(x => x.MontantPaye, "montant paye");

        RuleFor(x => x.DateEmission)
            .NotEmpty().WithMessage("La date d'emission est obligatoire");

        RuleFor(x => x.DateEcheance)
            .NotEmpty().WithMessage("La date d'echeance est obligatoire")
            .GreaterThanOrEqualTo(x => x.DateEmission).WithMessage("La date d'echeance doit etre posterieure ou egale a la date d'emission");

        RuleForEnum(x => x.StatutFacture, ["Emise", "PartPaye", "Payee", "Echue", "Annulee"], "statut facture");

        RuleForRequiredString(x => x.NumeroFacture, 50, "numero de facture");
    }
}
