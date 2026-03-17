namespace GnCache.Infrastructure.Models;

/// <summary>
/// Donnees de reference : Prefecture de Guinee.
/// </summary>
public sealed class Prefecture
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int RegionId { get; set; }
}
