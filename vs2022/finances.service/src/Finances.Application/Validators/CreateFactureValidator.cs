using Finances.Application.Commands;
using FluentValidation;
using GnValidation.FluentValidation;

namespace Finances.Application.Validators;

public sealed class CreateFactureValidator : BaseEntityValidator<CreateFactureCommand>
{
    public CreateFactureValidator()
    {
        RuleForRequiredFk(x => x.TypeId, "Type de facture");
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleForRequiredFk(x => x.AnneeScolaireId, "Annee scolaire");
        RuleForMoney(x => x.MontantTotal, "Montant total");
        RuleForMoney(x => x.MontantPaye, "Montant paye");
        RuleFor(x => x.DateEmission).NotEmpty().WithMessage("La date d'emission est obligatoire");
        RuleFor(x => x.DateEcheance).NotEmpty().WithMessage("La date d'echeance est obligatoire");
        RuleFor(x => x.DateEcheance).GreaterThanOrEqualTo(x => x.DateEmission)
            .WithMessage("La date d'echeance doit etre posterieure ou egale a la date d'emission");
        RuleForEnum(x => x.StatutFacture,
            ["Emise", "PartPaye", "Payee", "Echue", "Annulee"], "Statut de la facture");
        RuleForRequiredString(x => x.NumeroFacture, 50, "Numero de facture");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
