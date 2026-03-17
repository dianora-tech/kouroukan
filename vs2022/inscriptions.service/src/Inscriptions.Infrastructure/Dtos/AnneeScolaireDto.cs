using GnDapper.Entities;

namespace Inscriptions.Infrastructure.Dtos;

public sealed class AnneeScolaireDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public bool EstActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
