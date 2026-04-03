using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;
using GnDapper.Models;

namespace ServicesPremium.Infrastructure.Dtos;

/// <summary>
/// DTO pour la table services.ServicesParents.
/// </summary>
[Table("services.services_parents")]
public sealed class ServiceParentDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TypeId { get; set; }

    public string Code { get; set; } = string.Empty;
    public decimal Tarif { get; set; }
    public string Periodicite { get; set; } = string.Empty;
    public bool EstActif { get; set; }
    public int? PeriodeEssaiJours { get; set; }
    public bool TarifDegressif { get; set; }

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
