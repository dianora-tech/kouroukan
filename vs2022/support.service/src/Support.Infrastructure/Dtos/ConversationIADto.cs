using GnDapper.Attributes;
using GnDapper.Entities;

namespace Support.Infrastructure.Dtos;

[Table("support.conversations_ia")]
public sealed class ConversationIADto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public string? Titre { get; set; }
    public bool EstActive { get; set; }
    public int NombreMessages { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
