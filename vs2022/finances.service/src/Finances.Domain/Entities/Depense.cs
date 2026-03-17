using GnDapper.Entities;

namespace Finances.Domain.Entities;

/// <summary>
/// Depense de l'etablissement avec workflow de validation multi-niveaux.
/// Table : finances.depenses
/// </summary>
public class Depense : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public decimal Montant { get; set; }
    public string MotifDepense { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string BeneficiaireNom { get; set; } = string.Empty;
    public string? BeneficiaireTelephone { get; set; }
    public string? BeneficiaireNIF { get; set; }
    public string StatutDepense { get; set; } = string.Empty;
    public DateTime DateDemande { get; set; }
    public DateTime? DateValidation { get; set; }
    public int? ValidateurId { get; set; }
    public string? PieceJointeUrl { get; set; }
    public string NumeroJustificatif { get; set; } = string.Empty;
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
