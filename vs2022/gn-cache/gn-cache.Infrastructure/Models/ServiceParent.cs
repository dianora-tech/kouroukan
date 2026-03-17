namespace GnCache.Infrastructure.Models;

/// <summary>
/// Donnees de reference : Service premium pour les parents.
/// </summary>
public sealed class ServiceParent
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Tarif { get; set; }
    public string Periodicite { get; set; } = string.Empty;
    public bool EstActif { get; set; }
    public int? PeriodeEssaiJours { get; set; }
    public bool TarifDegressif { get; set; }
    public int? TypeId { get; set; }
}
