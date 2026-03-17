namespace GnCache.Infrastructure.Models;

/// <summary>
/// Donnees de reference : Sous-prefecture de Guinee.
/// </summary>
public sealed class SousPrefecture
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int PrefectureId { get; set; }
}
