using Inscriptions.Domain.Entities;
using Inscriptions.Infrastructure.Dtos;

namespace Inscriptions.Infrastructure.Mappers;

public static class LiaisonParentMapper
{
    public static LiaisonParent ToEntity(LiaisonParentDto dto)
    {
        return new LiaisonParent
        {
            Id = dto.Id,
            ParentUserId = dto.ParentUserId,
            EleveId = dto.EleveId,
            CompanyId = dto.CompanyId,
            Statut = dto.Statut,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static LiaisonParentDto ToDto(LiaisonParent entity)
    {
        return new LiaisonParentDto
        {
            Id = entity.Id,
            ParentUserId = entity.ParentUserId,
            EleveId = entity.EleveId,
            CompanyId = entity.CompanyId,
            Statut = entity.Statut,
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
