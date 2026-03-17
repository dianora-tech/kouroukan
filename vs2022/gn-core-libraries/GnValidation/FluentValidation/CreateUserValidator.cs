using GnValidation.Commands;
using GnValidation.Rules;
using FluentValidation;

namespace GnValidation.FluentValidation;

/// <summary>
/// Validateur pour la creation d'un utilisateur.
/// Valide l'email (RFC 5322), le telephone (format pays cible), et la force du mot de passe.
/// </summary>
public sealed class CreateUserValidator : BaseEntityValidator<CreateUserCommand>
{
    public CreateUserValidator(
        IPhoneNumberValidator phoneValidator,
        IPasswordStrengthValidator passwordValidator,
        IEmailValidator emailValidator)
    {
        ArgumentNullException.ThrowIfNull(phoneValidator, nameof(phoneValidator));
        ArgumentNullException.ThrowIfNull(passwordValidator, nameof(passwordValidator));
        ArgumentNullException.ThrowIfNull(emailValidator, nameof(emailValidator));

        RuleForRequiredString(x => x.FirstName, 100, "prenom");
        RuleFor(x => x.FirstName)
            .MinimumLength(2).WithMessage("Le prenom doit contenir au moins 2 caracteres");

        RuleForRequiredString(x => x.LastName, 100, "nom");
        RuleFor(x => x.LastName)
            .MinimumLength(2).WithMessage("Le nom doit contenir au moins 2 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est obligatoire")
            .MaximumLength(200).WithMessage("L'email ne peut pas depasser 200 caracteres")
            .Must(email => emailValidator.IsValid(email))
            .WithMessage("L'adresse email est invalide");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Le numero de telephone est obligatoire")
            .Must(phone => phoneValidator.IsValid(phone))
            .WithMessage("Le numero de telephone est invalide");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire")
            .Must(pwd => passwordValidator.Validate(pwd).IsValid)
            .WithMessage("Le mot de passe ne respecte pas les criteres de securite (min 8 caracteres, 1 majuscule, 1 minuscule, 1 chiffre, 1 caractere special)");
    }
}
