using GnDapper.Entities;

namespace Finances.Domain.Entities;

/// <summary>
/// Moyen de paiement configure pour un etablissement (Orange Money, etc.).
/// Table : finances.moyens_paiement
/// </summary>
public class MoyenPaiement : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers l'etablissement (auth.companies).</summary>
    public int CompanyId { get; set; }

    /// <summary>Operateur de paiement (OrangeMoney, MoovMoney, etc.).</summary>
    public string Operateur { get; set; } = string.Empty;

    /// <summary>Numero de compte marchand.</summary>
    public string NumeroCompte { get; set; } = string.Empty;

    /// <summary>Code marchand.</summary>
    public string CodeMarchand { get; set; } = string.Empty;

    /// <summary>Libelle du moyen de paiement.</summary>
    public string Libelle { get; set; } = string.Empty;

    /// <summary>Indique si le moyen de paiement est actif.</summary>
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
