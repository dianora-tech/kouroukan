using GnValidation.Commands.Personnel;
using GnValidation.Rules;
using FluentValidation;

namespace GnValidation.FluentValidation.Personnel;

/// <summary>
/// Validateur pour la creation d'un enseignant.
/// Verifie le nom, le matricule, la specialite, la date d'embauche,
/// le mode de remuneration, le montant forfait optionnel,
/// le telephone, l'email optionnel, le statut et le solde de conges.
/// </summary>
public sealed class CreateEnseignantValidator : BaseEntityValidator<CreateEnseignantCommand>
{
    public CreateEnseignantValidator(
        IPhoneNumberValidator phoneValidator,
        IEmailValidator emailValidator)
    {
        ArgumentNullException.ThrowIfNull(phoneValidator, nameof(phoneValidator));
        ArgumentNullException.ThrowIfNull(emailValidator, nameof(emailValidator));

        RuleForRequiredString(x => x.Name, 200, "nom");

        RuleForRequiredString(x => x.Matricule, 50, "matricule");

        RuleForRequiredString(x => x.Specialite, 200, "specialite");

        RuleFor(x => x.DateEmbauche)
            .NotEmpty().WithMessage("La date d'embauche est obligatoire");

        RuleForEnum(x => x.ModeRemuneration, ["Forfait", "Heures", "Mixte"], "mode de remuneration");

        RuleFor(x => x.MontantForfait)
            .GreaterThanOrEqualTo(0).WithMessage("Le montant forfait ne peut pas etre negatif")
            .When(x => x.MontantForfait.HasValue);

        RuleFor(x => x.Telephone)
            .NotEmpty().WithMessage("Le telephone est obligatoire")
            .Must(phone => phoneValidator.IsValid(phone)).WithMessage("Le numero de telephone est invalide");

        RuleFor(x => x.Email)
            .Must(email => emailValidator.IsValid(email!)).WithMessage("L'adresse email est invalide")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleForEnum(x => x.StatutEnseignant, ["Actif", "EnConge", "Suspendu", "Inactif"], "statut enseignant");

        RuleFor(x => x.SoldeCongesAnnuel)
            .GreaterThanOrEqualTo(0).WithMessage("Le solde de conges ne peut pas etre negatif");
    }
}
