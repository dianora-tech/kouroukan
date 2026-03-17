namespace GnCache.Infrastructure.Models;

/// <summary>
/// Donnees de reference : Region geographique de Guinee.
/// </summary>
public sealed class Region
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
