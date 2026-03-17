using GnDapper.Entities;

namespace Support.Domain.Entities;

/// <summary>
/// Vote d'un utilisateur pour une suggestion.
/// Contrainte UNIQUE(SuggestionId, VotantId).
/// </summary>
public class VoteSuggestion : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int SuggestionId { get; set; }
    public int VotantId { get; set; }

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
