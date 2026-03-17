using GnValidation.Options;
using Microsoft.Extensions.Options;
using PhoneNumbers;

namespace GnValidation.Rules;

/// <summary>
/// Implementation du validateur de numeros de telephone via libphonenumber.
/// Region par defaut configurable (defaut : "GN" pour la Guinee).
/// </summary>
public sealed class PhoneNumberValidator : IPhoneNumberValidator
{
    private readonly PhoneNumberUtil _phoneUtil;
    private readonly string _defaultRegionCode;

    /// <summary>
    /// Initialise le validateur avec les options de configuration.
    /// </summary>
    /// <param name="options">Options contenant le code region par defaut.</param>
    public PhoneNumberValidator(IOptions<GnValidationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        _phoneUtil = PhoneNumberUtil.GetInstance();
        _defaultRegionCode = options.Value.DefaultRegionCode;
    }

    /// <inheritdoc />
    public bool IsValid(string phoneNumber, string? regionCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        try
        {
            var region = regionCode ?? _defaultRegionCode;
            var parsed = _phoneUtil.Parse(phoneNumber, region);
            return _phoneUtil.IsValidNumber(parsed);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public string? Format(string phoneNumber, string? regionCode = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var region = regionCode ?? _defaultRegionCode;
            var parsed = _phoneUtil.Parse(phoneNumber, region);

            if (!_phoneUtil.IsValidNumber(parsed))
                return null;

            return _phoneUtil.Format(parsed, PhoneNumberFormat.E164);
        }
        catch (NumberParseException)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public string? GetRegionCode(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var parsed = _phoneUtil.Parse(phoneNumber, _defaultRegionCode);
            var region = _phoneUtil.GetRegionCodeForNumber(parsed);
            return region;
        }
        catch (NumberParseException)
        {
            return null;
        }
    }
}
