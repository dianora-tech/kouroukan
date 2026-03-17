using FluentValidation;
using GnValidation.FluentValidation;
using Personnel.Application.Commands;

namespace Personnel.Application.Validators;

public sealed class CreateDemandeCongeValidator : BaseEntityValidator<CreateDemandeCongeCommand>
{
    public CreateDemandeCongeValidator()
    {
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForOptionalString(x => x.Description, 500, "Description");
        RuleForRequiredFk(x => x.EnseignantId, "Enseignant");
        RuleFor(x => x.DateDebut).NotEmpty().WithMessage("La date de debut est obligatoire");
        RuleFor(x => x.DateFin).NotEmpty().WithMessage("La date de fin est obligatoire");
        RuleFor(x => x.DateFin).GreaterThan(x => x.DateDebut).WithMessage("La date de fin doit etre posterieure a la date de debut");
        RuleForRequiredString(x => x.Motif, 500, "Motif");
        RuleForEnum(x => x.StatutDemande, ["Soumise", "ApprouveeN1", "ApprouveeDirection", "Refusee"], "Statut demande");
        RuleForUrl(x => x.PieceJointeUrl, "Piece jointe URL");
        RuleForOptionalString(x => x.CommentaireValidateur, 500, "Commentaire validateur");
        RuleForRequiredFk(x => x.TypeId, "Type demande conge");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
