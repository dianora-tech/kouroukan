using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Finances.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table finances.moyens_paiement.
/// </summary>
[Table("finances.moyens_paiement")]
public sealed class MoyenPaiementDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Operateur { get; set; } = string.Empty;
    public string NumeroCompte { get; set; } = string.Empty;
    public string CodeMarchand { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public bool EstActif { get; set; }

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
