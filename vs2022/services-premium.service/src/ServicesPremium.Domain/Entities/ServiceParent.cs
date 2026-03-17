using GnDapper.Entities;
using GnDapper.Models;

namespace ServicesPremium.Domain.Entities;

/// <summary>
/// Service a valeur ajoutee propose aux parents (SMS, alertes, contenus pedagogiques, etc.).
/// </summary>
public class ServiceParent : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TypeId { get; set; }

    /// <summary>Code unique du service.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Tarif du service.</summary>
    public decimal Tarif { get; set; }

    /// <summary>Periodicite : Mensuel, Annuel, Unite.</summary>
    public string Periodicite { get; set; } = string.Empty;

    /// <summary>Service actif ou non.</summary>
    public bool EstActif { get; set; }

    /// <summary>Duree de l'essai gratuit en jours.</summary>
    public int? PeriodeEssaiJours { get; set; }

    /// <summary>Tarif degressif par enfant.</summary>
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
