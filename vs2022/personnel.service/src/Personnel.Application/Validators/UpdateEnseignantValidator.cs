using FluentValidation;
using GnValidation.FluentValidation;
using Personnel.Application.Commands;

namespace Personnel.Application.Validators;

public sealed class UpdateEnseignantValidator : BaseEntityValidator<UpdateEnseignantCommand>
{
    public UpdateEnseignantValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForOptionalString(x => x.Description, 500, "Description");
        RuleForRequiredString(x => x.Matricule, 50, "Matricule");
        RuleForRequiredString(x => x.Specialite, 200, "Specialite");
        RuleFor(x => x.DateEmbauche).NotEmpty().WithMessage("La date d'embauche est obligatoire");
        RuleForEnum(x => x.ModeRemuneration, ["Forfait", "Heures", "Mixte"], "Mode de remuneration");
        RuleFor(x => x.MontantForfait)
            .GreaterThanOrEqualTo(0).WithMessage("Le montant forfait ne peut pas etre negatif")
            .When(x => x.MontantForfait.HasValue);
        RuleForRequiredString(x => x.Telephone, 20, "Telephone");
        RuleForOptionalString(x => x.Email, 200, "Email");
        RuleForEnum(x => x.StatutEnseignant, ["Actif", "EnConge", "Suspendu", "Inactif"], "Statut enseignant");
        RuleFor(x => x.SoldeCongesAnnuel).GreaterThanOrEqualTo(0).WithMessage("Le solde de conges doit etre positif ou nul");
        RuleForRequiredFk(x => x.TypeId, "Type enseignant");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
