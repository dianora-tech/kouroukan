using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;
using GnDapper.Models;

namespace ServicesPremium.Infrastructure.Dtos;

/// <summary>
/// DTO pour la table services.Souscriptions.
/// </summary>
[Table("services.souscriptions")]
public sealed class SouscriptionDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    public int ServiceParentId { get; set; }
    public int ParentId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string StatutSouscription { get; set; } = string.Empty;
    public decimal MontantPaye { get; set; }
    public bool RenouvellementAuto { get; set; }
    public DateTime? DateProchainRenouvellement { get; set; }

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
