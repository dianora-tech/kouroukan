using GnDapper.Entities;

namespace Support.Domain.Entities;

/// <summary>
/// Conversation d'aide generative IA avec un utilisateur.
/// </summary>
public class ConversationIA : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int UtilisateurId { get; set; }
    public string? Titre { get; set; }
    public bool EstActive { get; set; }
    public int NombreMessages { get; set; }

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
