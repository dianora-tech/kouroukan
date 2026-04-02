using Finances.Domain.Entities;
using Finances.Infrastructure.Dtos;

namespace Finances.Infrastructure.Mappers;

public static class PalierFamilialMapper
{
    public static PalierFamilial ToEntity(PalierFamilialDto dto)
    {
        return new PalierFamilial
        {
            Id = dto.Id,
            CompanyId = dto.CompanyId,
            RangEnfant = dto.RangEnfant,
            ReductionPourcent = dto.ReductionPourcent,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static PalierFamilialDto ToDto(PalierFamilial entity)
    {
        return new PalierFamilialDto
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            RangEnfant = entity.RangEnfant,
            ReductionPourcent = entity.ReductionPourcent,
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
