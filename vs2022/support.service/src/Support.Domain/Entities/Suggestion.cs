using GnDapper.Entities;

namespace Support.Domain.Entities;

/// <summary>
/// Suggestion d'amelioration soumise par un utilisateur.
/// </summary>
public class Suggestion : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TypeId { get; set; }

    public int AuteurId { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public string? ModuleConcerne { get; set; }
    public string StatutSuggestion { get; set; } = string.Empty;
    public int NombreVotes { get; set; }
    public string? CommentaireAdmin { get; set; }

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
