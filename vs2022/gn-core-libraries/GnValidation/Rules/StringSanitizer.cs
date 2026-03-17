using System.Text.RegularExpressions;

namespace GnValidation.Rules;

/// <summary>
/// Implementation du service de nettoyage de chaines de caracteres.
/// Utilise des regex source-generated pour la performance.
/// </summary>
public sealed partial class StringSanitizer : IStringSanitizer
{
    /// <inheritdoc />
    public string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var result = StripHtml(input);
        result = NormalizeWhitespace(result);
        return result.Trim();
    }

    /// <inheritdoc />
    public string StripHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return HtmlTagRegex().Replace(input, string.Empty);
    }

    /// <inheritdoc />
    public string SanitizeForSql(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var result = input
            .Replace("'", "''")
            .Replace("--", "")
            .Replace(";", "")
            .Replace("/*", "")
            .Replace("*/", "");

        return result;
    }

    /// <inheritdoc />
    public string NormalizeWhitespace(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return WhitespaceRegex().Replace(input, " ");
    }

    [GeneratedRegex(@"<[^>]*>", RegexOptions.Compiled)]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();
}
