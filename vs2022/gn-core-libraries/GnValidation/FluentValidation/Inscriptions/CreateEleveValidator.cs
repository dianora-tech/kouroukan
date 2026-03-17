using GnValidation.Commands.Inscriptions;
using FluentValidation;

namespace GnValidation.FluentValidation.Inscriptions;

/// <summary>
/// Validateur pour la creation d'un eleve.
/// Verifie les informations personnelles, le matricule et le rattachement a une classe.
/// </summary>
public sealed class CreateEleveValidator : BaseEntityValidator<CreateEleveCommand>
{
    public CreateEleveValidator()
    {
        RuleForRequiredString(x => x.FirstName, 100, "prenom");

        RuleForRequiredString(x => x.LastName, 100, "nom");

        RuleFor(x => x.DateNaissance)
            .NotEmpty().WithMessage("La date de naissance est obligatoire")
            .LessThan(DateTime.Today).WithMessage("La date de naissance doit etre dans le passe");

        RuleForRequiredString(x => x.LieuNaissance, 200, "lieu de naissance");

        RuleForEnum(x => x.Genre, ["M", "F"], "genre");

        RuleForRequiredString(x => x.Nationalite, 50, "nationalite");

        RuleForOptionalString(x => x.Adresse, 500, "adresse");

        RuleForUrl(x => x.PhotoUrl, "photo");

        RuleForRequiredString(x => x.NumeroMatricule, 50, "matricule");

        RuleForRequiredFk(x => x.NiveauClasseId, "niveau de classe");

        RuleFor(x => x.ClasseId)
            .GreaterThan(0).WithMessage("L'identifiant de la classe doit etre superieur a 0")
            .When(x => x.ClasseId.HasValue);

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("L'identifiant du parent doit etre superieur a 0")
            .When(x => x.ParentId.HasValue);

        RuleForEnum(x => x.StatutInscription, ["Prospect", "PreInscrit", "Inscrit", "Radie"], "statut inscription");
    }
}
