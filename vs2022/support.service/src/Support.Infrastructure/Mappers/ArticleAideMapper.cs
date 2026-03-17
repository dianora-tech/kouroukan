using Support.Domain.Entities;
using Support.Infrastructure.Dtos;

namespace Support.Infrastructure.Mappers;

public static class ArticleAideMapper
{
    public static ArticleAide ToEntity(ArticleAideDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        TypeId = dto.TypeId,
        Titre = dto.Titre,
        Contenu = dto.Contenu,
        Slug = dto.Slug,
        Categorie = dto.Categorie,
        ModuleConcerne = dto.ModuleConcerne,
        Ordre = dto.Ordre,
        EstPublie = dto.EstPublie,
        NombreVues = dto.NombreVues,
        NombreUtile = dto.NombreUtile,
        UserId = dto.UserId,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        CreatedBy = dto.CreatedBy,
        UpdatedBy = dto.UpdatedBy,
        IsDeleted = dto.IsDeleted,
        DeletedAt = dto.DeletedAt,
        DeletedBy = dto.DeletedBy
    };

    public static ArticleAideDto ToDto(ArticleAide entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        TypeId = entity.TypeId,
        Titre = entity.Titre,
        Contenu = entity.Contenu,
        Slug = entity.Slug,
        Categorie = entity.Categorie,
        ModuleConcerne = entity.ModuleConcerne,
        Ordre = entity.Ordre,
        EstPublie = entity.EstPublie,
        NombreVues = entity.NombreVues,
        NombreUtile = entity.NombreUtile,
        UserId = entity.UserId,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedBy = entity.UpdatedBy,
        IsDeleted = entity.IsDeleted,
        DeletedAt = entity.DeletedAt,
        DeletedBy = entity.DeletedBy
    };
}
