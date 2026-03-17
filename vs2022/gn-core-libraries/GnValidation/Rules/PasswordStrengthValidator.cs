using GnValidation.Models;
using GnValidation.Options;
using Microsoft.Extensions.Options;

namespace GnValidation.Rules;

/// <summary>
/// Implementation du validateur de force de mot de passe.
/// Criteres : longueur minimale (defaut 8), 1 majuscule, 1 minuscule, 1 chiffre, 1 caractere special.
/// </summary>
public sealed class PasswordStrengthValidator : IPasswordStrengthValidator
{
    private readonly int _minLength;

    /// <summary>
    /// Initialise le validateur avec les options de configuration.
    /// </summary>
    /// <param name="options">Options contenant la longueur minimale du mot de passe.</param>
    public PasswordStrengthValidator(IOptions<GnValidationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        _minLength = options.Value.PasswordMinLength;
    }

    /// <inheritdoc />
    public PasswordStrengthResult Validate(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return new PasswordStrengthResult(false, 0, new List<string>
            {
                $"Le mot de passe doit contenir au moins {_minLength} caracteres",
                "Ajoutez au moins une lettre majuscule",
                "Ajoutez au moins une lettre minuscule",
                "Ajoutez au moins un chiffre",
                "Ajoutez au moins un caractere special (!@#$%^&*)"
            });
        }

        var score = 0;
        var suggestions = new List<string>();

        // Critere 1 : longueur minimale
        if (password.Length >= _minLength)
            score++;
        else
            suggestions.Add($"Le mot de passe doit contenir au moins {_minLength} caracteres");

        // Critere 2 : au moins une majuscule
        if (password.Any(char.IsUpper))
            score++;
        else
            suggestions.Add("Ajoutez au moins une lettre majuscule");

        // Critere 3 : au moins une minuscule
        if (password.Any(char.IsLower))
            score++;
        else
            suggestions.Add("Ajoutez au moins une lettre minuscule");

        // Critere 4 : au moins un chiffre
        if (password.Any(char.IsDigit))
            score++;
        else
            suggestions.Add("Ajoutez au moins un chiffre");

        // Critere 5 : au moins un caractere special
        if (password.Any(c => !char.IsLetterOrDigit(c)))
            score++;
        else
            suggestions.Add("Ajoutez au moins un caractere special (!@#$%^&*)");

        var isValid = score == 5;
        return new PasswordStrengthResult(isValid, score, suggestions);
    }
}
