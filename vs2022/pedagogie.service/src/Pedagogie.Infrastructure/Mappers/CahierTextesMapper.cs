using Pedagogie.Domain.Entities;
using Pedagogie.Infrastructure.Dtos;

namespace Pedagogie.Infrastructure.Mappers;

public static class CahierTextesMapper
{
    public static CahierTextes ToEntity(CahierTextesDto dto)
    {
        return new CahierTextes
        {
            Id = dto.Id,
            SeanceId = dto.SeanceId,
            Contenu = dto.Contenu,
            DateSeance = dto.DateSeance,
            TravailAFaire = dto.TravailAFaire,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static CahierTextesDto ToDto(CahierTextes entity)
    {
        return new CahierTextesDto
        {
            Id = entity.Id,
            SeanceId = entity.SeanceId,
            Contenu = entity.Contenu,
            DateSeance = entity.DateSeance,
            TravailAFaire = entity.TravailAFaire,
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
