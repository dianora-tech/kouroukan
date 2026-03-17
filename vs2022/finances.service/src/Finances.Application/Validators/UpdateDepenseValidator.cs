using Finances.Application.Commands;
using FluentValidation;
using GnValidation.FluentValidation;

namespace Finances.Application.Validators;

public sealed class UpdateDepenseValidator : BaseEntityValidator<UpdateDepenseCommand>
{
    public UpdateDepenseValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredFk(x => x.TypeId, "Type de depense");
        RuleFor(x => x.Montant).GreaterThan(0).WithMessage("Le montant doit etre superieur a zero");
        RuleForRequiredString(x => x.MotifDepense, 500, "Motif de la depense");
        RuleForEnum(x => x.Categorie,
            ["Personnel", "Fournitures", "Maintenance", "Evenements", "BDE", "Equipements"], "Categorie");
        RuleForRequiredString(x => x.BeneficiaireNom, 200, "Nom du beneficiaire");
        RuleForOptionalString(x => x.BeneficiaireTelephone, 20, "Telephone du beneficiaire");
        RuleForOptionalString(x => x.BeneficiaireNIF, 50, "NIF du beneficiaire");
        RuleForEnum(x => x.StatutDepense,
            ["Demande", "ValideN1", "ValideFinance", "ValideDirection", "Executee", "Archivee"], "Statut de la depense");
        RuleFor(x => x.DateDemande).NotEmpty().WithMessage("La date de demande est obligatoire");
        RuleForUrl(x => x.PieceJointeUrl, "Piece jointe");
        RuleForRequiredString(x => x.NumeroJustificatif, 50, "Numero de justificatif");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
