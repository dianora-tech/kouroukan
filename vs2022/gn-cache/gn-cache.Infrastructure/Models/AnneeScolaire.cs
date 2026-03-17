namespace GnCache.Infrastructure.Models;

/// <summary>
/// Donnees de reference : Annee scolaire.
/// </summary>
public sealed class AnneeScolaire
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public bool EstActive { get; set; }
}
