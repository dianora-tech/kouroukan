namespace GnCache.Infrastructure.Models;

/// <summary>
/// Donnees de reference : Niveau de classe du systeme educatif guineen.
/// </summary>
public sealed class NiveauClasse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Ordre { get; set; }
    public string CycleEtude { get; set; } = string.Empty;
    public int? AgeOfficielEntree { get; set; }
    public string? MinistereTutelle { get; set; }
    public string? ExamenSortie { get; set; }
    public decimal? TauxHoraireEnseignant { get; set; }
}
