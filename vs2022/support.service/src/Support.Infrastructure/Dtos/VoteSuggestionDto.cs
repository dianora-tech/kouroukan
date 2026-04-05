using GnDapper.Attributes;
using GnDapper.Entities;

namespace Support.Infrastructure.Dtos;

[Table("support.votes_suggestions")]
public sealed class VoteSuggestionDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int SuggestionId { get; set; }
    public int VotantId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
