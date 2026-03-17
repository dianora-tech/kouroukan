namespace GnValidation.Rules;

/// <summary>
/// Validateur de numeros de telephone internationaux via libphonenumber.
/// </summary>
public interface IPhoneNumberValidator
{
    /// <summary>
    /// Verifie si un numero de telephone est valide pour une region donnee.
    /// Si <paramref name="regionCode"/> est null, utilise la region par defaut configuree.
    /// </summary>
    /// <param name="phoneNumber">Numero de telephone a valider.</param>
    /// <param name="regionCode">Code ISO 3166-1 alpha-2 de la region (ex: "GN", "FR"). Null = region par defaut.</param>
    /// <returns><c>true</c> si le numero est valide, <c>false</c> sinon.</returns>
    bool IsValid(string phoneNumber, string? regionCode = null);

    /// <summary>
    /// Formate un numero de telephone au format international E.164.
    /// </summary>
    /// <param name="phoneNumber">Numero de telephone a formater.</param>
    /// <param name="regionCode">Code ISO 3166-1 alpha-2 de la region. Null = region par defaut.</param>
    /// <returns>Numero au format E.164 (ex: "+224621000000"), ou null si le numero est invalide.</returns>
    string? Format(string phoneNumber, string? regionCode = null);

    /// <summary>
    /// Detecte le code region d'un numero de telephone.
    /// </summary>
    /// <param name="phoneNumber">Numero de telephone au format international.</param>
    /// <returns>Code ISO 3166-1 alpha-2 (ex: "GN"), ou null si non detecte.</returns>
    string? GetRegionCode(string phoneNumber);
}
