using GnValidation.Options;
using Microsoft.Extensions.Options;

namespace GnValidation.Rules;

/// <summary>
/// Implementation du validateur de coordonnees GPS.
/// Bounding box par defaut : Guinee (lat 7.19-12.68, lon -15.08 a -7.64).
/// </summary>
public sealed class CoordinatesValidator : ICoordinatesValidator
{
    private readonly double _latMin;
    private readonly double _latMax;
    private readonly double _lonMin;
    private readonly double _lonMax;

    /// <summary>
    /// Initialise le validateur avec les options de configuration du bounding box.
    /// </summary>
    /// <param name="options">Options contenant les limites du bounding box.</param>
    public CoordinatesValidator(IOptions<GnValidationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        _latMin = options.Value.LatitudeMin;
        _latMax = options.Value.LatitudeMax;
        _lonMin = options.Value.LongitudeMin;
        _lonMax = options.Value.LongitudeMax;
    }

    /// <inheritdoc />
    public bool IsValidLatitude(double latitude)
    {
        return latitude is >= -90.0 and <= 90.0;
    }

    /// <inheritdoc />
    public bool IsValidLongitude(double longitude)
    {
        return longitude is >= -180.0 and <= 180.0;
    }

    /// <inheritdoc />
    public bool IsInCountry(double latitude, double longitude)
    {
        if (!IsValidLatitude(latitude) || !IsValidLongitude(longitude))
            return false;

        return latitude >= _latMin
            && latitude <= _latMax
            && longitude >= _lonMin
            && longitude <= _lonMax;
    }
}
