using FluentValidation;
using GnValidation.FluentValidation;
using Inscriptions.Application.Commands;

namespace Inscriptions.Application.Validators;

public sealed class UpdateEleveValidator : BaseEntityValidator<UpdateEleveCommand>
{
    public UpdateEleveValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredString(x => x.FirstName, 100, "Prenom");
        RuleForRequiredString(x => x.LastName, 100, "Nom");
        RuleFor(x => x.DateNaissance).NotEmpty().WithMessage("La date de naissance est obligatoire");
        RuleForRequiredString(x => x.LieuNaissance, 200, "Lieu de naissance");
        RuleForEnum(x => x.Genre, ["M", "F"], "Genre");
        RuleForRequiredString(x => x.Nationalite, 50, "Nationalite");
        RuleForOptionalString(x => x.Adresse, 500, "Adresse");
        RuleForUrl(x => x.PhotoUrl, "Photo URL");
        RuleForRequiredString(x => x.NumeroMatricule, 50, "Numero matricule");
        RuleForRequiredFk(x => x.NiveauClasseId, "Niveau classe");
        RuleForEnum(x => x.StatutInscription, ["Prospect", "PreInscrit", "Inscrit", "Radie"], "Statut inscription");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
