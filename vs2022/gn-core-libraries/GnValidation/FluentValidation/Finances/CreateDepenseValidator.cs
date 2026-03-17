using GnValidation.Commands.Finances;
using FluentValidation;

namespace GnValidation.FluentValidation.Finances;

/// <summary>
/// Validateur pour la creation d'une depense.
/// Verifie le montant, le motif, la categorie, les informations du beneficiaire,
/// le statut, la date de demande, l'URL de piece jointe et le numero de justificatif.
/// </summary>
public sealed class CreateDepenseValidator : BaseEntityValidator<CreateDepenseCommand>
{
    public CreateDepenseValidator()
    {
        RuleFor(x => x.Montant)
            .GreaterThan(0).WithMessage("Le montant de la depense doit etre superieur a 0");

        RuleForRequiredString(x => x.MotifDepense, 500, "motif de depense");

        RuleForEnum(x => x.Categorie, ["Personnel", "Fournitures", "Maintenance", "Evenements", "BDE", "Equipements"], "categorie");

        RuleForRequiredString(x => x.BeneficiaireNom, 200, "nom du beneficiaire");

        RuleForOptionalString(x => x.BeneficiaireTelephone, 20, "telephone du beneficiaire");

        RuleForOptionalString(x => x.BeneficiaireNIF, 50, "NIF du beneficiaire");

        RuleForEnum(x => x.StatutDepense, ["Demande", "ValideN1", "ValideFinance", "ValideDirection", "Executee", "Archivee"], "statut depense");

        RuleFor(x => x.DateDemande)
            .NotEmpty().WithMessage("La date de demande est obligatoire");

        RuleForUrl(x => x.PieceJointeUrl, "piece jointe");

        RuleForRequiredString(x => x.NumeroJustificatif, 50, "numero de justificatif");
    }
}
