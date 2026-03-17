namespace GnValidation.Rules;

/// <summary>
/// Validateur d'adresses email conforme RFC 5322.
/// </summary>
public interface IEmailValidator
{
    /// <summary>
    /// Verifie si une adresse email est valide selon RFC 5322.
    /// </summary>
    /// <param name="email">Adresse email a valider.</param>
    /// <returns><c>true</c> si l'adresse est valide, <c>false</c> sinon.</returns>
    bool IsValid(string email);
}
