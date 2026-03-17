namespace GnValidation.Rules;

/// <summary>
/// Service de nettoyage et assainissement de chaines de caracteres.
/// Protege contre les injections XSS, SQL et normalise les espaces.
/// </summary>
public interface IStringSanitizer
{
    /// <summary>
    /// Nettoie une chaine : supprime les balises HTML, normalise les espaces et trim.
    /// </summary>
    /// <param name="input">Chaine a nettoyer.</param>
    /// <returns>Chaine nettoyee.</returns>
    string Sanitize(string input);

    /// <summary>
    /// Supprime toutes les balises HTML d'une chaine.
    /// </summary>
    /// <param name="input">Chaine contenant potentiellement du HTML.</param>
    /// <returns>Chaine sans balises HTML.</returns>
    string StripHtml(string input);

    /// <summary>
    /// Echappe les caracteres dangereux pour les requetes SQL.
    /// Note : privilegier les requetes parametrees. Ce nettoyage est une couche de defense supplementaire.
    /// </summary>
    /// <param name="input">Chaine a assainir.</param>
    /// <returns>Chaine avec les caracteres SQL dangereux echappes.</returns>
    string SanitizeForSql(string input);

    /// <summary>
    /// Remplace les espaces multiples (espaces, tabulations, retours a la ligne) par un seul espace.
    /// </summary>
    /// <param name="input">Chaine avec des espaces multiples.</param>
    /// <returns>Chaine avec les espaces normalises.</returns>
    string NormalizeWhitespace(string input);
}
