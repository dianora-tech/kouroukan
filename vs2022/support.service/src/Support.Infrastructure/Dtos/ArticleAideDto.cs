using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Support.Infrastructure.Dtos;

[Table("support.articles_aide")]
public sealed class ArticleAideDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string? ModuleConcerne { get; set; }
    public int Ordre { get; set; }
    public bool EstPublie { get; set; }
    public int NombreVues { get; set; }
    public int NombreUtile { get; set; }
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
