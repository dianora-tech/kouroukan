namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// DTO representant la version active des CGU.
/// </summary>
public class CguVersionDto
{
    /// <summary>Numero de version (ex: "1.0").</summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>Contenu des CGU en Markdown.</summary>
    public string Contenu { get; set; } = string.Empty;

    /// <summary>Date de publication.</summary>
    public DateTime DatePublication { get; set; }
}
