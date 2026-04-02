using Pedagogie.Domain.Entities;
using Pedagogie.Infrastructure.Dtos;

namespace Pedagogie.Infrastructure.Mappers;

public static class MatiereMapper
{
    public static Matiere ToEntity(MatiereDto dto)
    {
        return new Matiere
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            TypeId = dto.TypeId,
            Code = dto.Code,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static MatiereDto ToDto(Matiere entity)
    {
        return new MatiereDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            TypeId = entity.TypeId,
            Code = entity.Code,
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
