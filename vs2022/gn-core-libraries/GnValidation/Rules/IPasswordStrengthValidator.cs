using GnValidation.Models;

namespace GnValidation.Rules;

/// <summary>
/// Validateur de force de mot de passe.
/// Criteres : longueur minimale, majuscule, minuscule, chiffre, caractere special.
/// </summary>
public interface IPasswordStrengthValidator
{
    /// <summary>
    /// Analyse la force d'un mot de passe et retourne un resultat detaille.
    /// </summary>
    /// <param name="password">Mot de passe a analyser.</param>
    /// <returns>Resultat contenant la validite, le score (0-5) et les suggestions en francais.</returns>
    PasswordStrengthResult Validate(string password);
}
