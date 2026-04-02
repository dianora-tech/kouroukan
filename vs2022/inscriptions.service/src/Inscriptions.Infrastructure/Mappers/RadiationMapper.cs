using Inscriptions.Domain.Entities;
using Inscriptions.Infrastructure.Dtos;

namespace Inscriptions.Infrastructure.Mappers;

public static class RadiationMapper
{
    public static Radiation ToEntity(RadiationDto dto)
    {
        return new Radiation
        {
            Id = dto.Id,
            EleveId = dto.EleveId,
            CompanyId = dto.CompanyId,
            Motif = dto.Motif,
            Documents = dto.Documents,
            DateRadiation = dto.DateRadiation,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static RadiationDto ToDto(Radiation entity)
    {
        return new RadiationDto
        {
            Id = entity.Id,
            EleveId = entity.EleveId,
            CompanyId = entity.CompanyId,
            Motif = entity.Motif,
            Documents = entity.Documents,
            DateRadiation = entity.DateRadiation,
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
