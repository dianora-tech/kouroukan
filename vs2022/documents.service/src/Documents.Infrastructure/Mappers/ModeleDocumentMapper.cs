using Documents.Domain.Entities;
using Documents.Infrastructure.Dtos;

namespace Documents.Infrastructure.Mappers;

public static class ModeleDocumentMapper
{
    public static ModeleDocument ToEntity(ModeleDocumentDto dto)
    {
        return new ModeleDocument
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            Code = dto.Code,
            Contenu = dto.Contenu,
            LogoUrl = dto.LogoUrl,
            CouleurPrimaire = dto.CouleurPrimaire,
            TextePiedPage = dto.TextePiedPage,
            EstActif = dto.EstActif,
            UserId = dto.UserId,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static ModeleDocumentDto ToDto(ModeleDocument entity)
    {
        return new ModeleDocumentDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            Code = entity.Code,
            Contenu = entity.Contenu,
            LogoUrl = entity.LogoUrl,
            CouleurPrimaire = entity.CouleurPrimaire,
            TextePiedPage = entity.TextePiedPage,
            EstActif = entity.EstActif,
            UserId = entity.UserId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.UpdatedBy,
            IsDeleted = entity.IsDeleted,
            DeletedAt = entity.DeletedAt,
            DeletedBy = entity.DeletedBy,
        };
    }
}
