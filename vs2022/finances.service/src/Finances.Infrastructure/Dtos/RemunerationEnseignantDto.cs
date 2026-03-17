using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Finances.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table finances.remunerations_enseignants.
/// </summary>
[Table("finances.remunerations_enseignants")]
public sealed class RemunerationEnseignantDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int EnseignantId { get; set; }
    public int Mois { get; set; }
    public int Annee { get; set; }
    public string ModeRemuneration { get; set; } = string.Empty;
    public decimal? MontantForfait { get; set; }
    public decimal? NombreHeures { get; set; }
    public decimal? TauxHoraire { get; set; }
    public decimal MontantTotal { get; set; }
    public string StatutPaiement { get; set; } = string.Empty;
    public DateTime? DateValidation { get; set; }
    public int? ValidateurId { get; set; }
    public int UserId { get; set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
