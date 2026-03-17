namespace GnValidation.Rules;

/// <summary>
/// Validateur de coordonnees GPS avec verification de bounding box pays.
/// </summary>
public interface ICoordinatesValidator
{
    /// <summary>
    /// Verifie si une latitude est dans la plage valide [-90, 90].
    /// </summary>
    /// <param name="latitude">Latitude a valider.</param>
    /// <returns><c>true</c> si la latitude est valide.</returns>
    bool IsValidLatitude(double latitude);

    /// <summary>
    /// Verifie si une longitude est dans la plage valide [-180, 180].
    /// </summary>
    /// <param name="longitude">Longitude a valider.</param>
    /// <returns><c>true</c> si la longitude est valide.</returns>
    bool IsValidLongitude(double longitude);

    /// <summary>
    /// Verifie si des coordonnees sont situees dans le pays cible (bounding box configurable).
    /// </summary>
    /// <param name="latitude">Latitude.</param>
    /// <param name="longitude">Longitude.</param>
    /// <returns><c>true</c> si les coordonnees sont dans le bounding box du pays.</returns>
    bool IsInCountry(double latitude, double longitude);
}
