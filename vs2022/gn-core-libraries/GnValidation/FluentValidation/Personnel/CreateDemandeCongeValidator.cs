using GnValidation.Commands.Personnel;
using FluentValidation;

namespace GnValidation.FluentValidation.Personnel;

/// <summary>
/// Validateur pour la creation d'une demande de conge.
/// Verifie la reference enseignant, les dates de debut et de fin,
/// le motif, le statut de la demande et l'URL de piece jointe.
/// </summary>
public sealed class CreateDemandeCongeValidator : BaseEntityValidator<CreateDemandeCongeCommand>
{
    public CreateDemandeCongeValidator()
    {
        RuleForRequiredFk(x => x.EnseignantId, "enseignant");

        RuleFor(x => x.DateDebut)
            .NotEmpty().WithMessage("La date de debut est obligatoire");

        RuleFor(x => x.DateFin)
            .NotEmpty().WithMessage("La date de fin est obligatoire")
            .GreaterThan(x => x.DateDebut).WithMessage("La date de fin doit etre posterieure a la date de debut");

        RuleForRequiredString(x => x.Motif, 500, "motif");

        RuleForEnum(x => x.StatutDemande, ["Soumise", "ApprouveeN1", "ApprouveeDirection", "Refusee"], "statut demande");

        RuleForUrl(x => x.PieceJointeUrl, "piece jointe");
    }
}
