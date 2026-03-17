namespace GnValidation.Options;

/// <summary>
/// Options de configuration pour la validation GnValidation.
/// Section de configuration : "GnValidation".
/// </summary>
public class GnValidationOptions
{
    /// <summary>Code ISO 3166-1 alpha-2 du pays cible pour la validation telephonique. Defaut : "GN" (Guinee).</summary>
    public string DefaultRegionCode { get; set; } = "GN";

    /// <summary>Latitude minimale du bounding box pays. Defaut : 7.19 (Guinee).</summary>
    public double LatitudeMin { get; set; } = 7.19;

    /// <summary>Latitude maximale du bounding box pays. Defaut : 12.68 (Guinee).</summary>
    public double LatitudeMax { get; set; } = 12.68;

    /// <summary>Longitude minimale du bounding box pays. Defaut : -15.08 (Guinee).</summary>
    public double LongitudeMin { get; set; } = -15.08;

    /// <summary>Longitude maximale du bounding box pays. Defaut : -7.64 (Guinee).</summary>
    public double LongitudeMax { get; set; } = -7.64;

    /// <summary>Longueur minimale du mot de passe. Defaut : 8.</summary>
    public int PasswordMinLength { get; set; } = 8;
}
