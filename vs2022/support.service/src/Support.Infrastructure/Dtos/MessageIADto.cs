using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Support.Infrastructure.Dtos;

[Table("support.messages_ia")]
public sealed class MessageIADto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int ConversationIAId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public string? ContexteArticlesIds { get; set; }
    public int? TokensUtilises { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
