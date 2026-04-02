using GnDapper.Entities;

namespace Finances.Domain.Entities;

/// <summary>
/// Entree dans le journal financier (mouvement de tresorerie).
/// Table : finances.journal_financier
/// </summary>
public class JournalFinancier : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers l'etablissement (auth.companies).</summary>
    public int CompanyId { get; set; }

    /// <summary>Type d'operation (Recette, Depense).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Categorie de l'operation (Scolarite, Cantine, Transport, etc.).</summary>
    public string Categorie { get; set; } = string.Empty;

    /// <summary>Montant de l'operation.</summary>
    public decimal Montant { get; set; }

    /// <summary>Description de l'operation.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Reference externe (numero de transaction mobile money, etc.).</summary>
    public string? ReferenceExterne { get; set; }

    /// <summary>Mode de paiement (Especes, OrangeMoney, MoovMoney, etc.).</summary>
    public string ModePaiement { get; set; } = string.Empty;

    /// <summary>Date de l'operation.</summary>
    public DateTime DateOperation { get; set; }

    /// <summary>FK vers l'eleve concerne (inscriptions.eleves).</summary>
    public int? EleveId { get; set; }

    /// <summary>FK vers le parent ayant effectue le paiement (auth.users).</summary>
    public int? ParentUserId { get; set; }

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
