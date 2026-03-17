namespace GnCache.Infrastructure.Models;

/// <summary>
/// Donnees de reference : Matiere scolaire.
/// </summary>
public sealed class Matiere
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Coefficient { get; set; }
    public int NombreHeures { get; set; }
    public int NiveauClasseId { get; set; }
    public int? TypeId { get; set; }
}
