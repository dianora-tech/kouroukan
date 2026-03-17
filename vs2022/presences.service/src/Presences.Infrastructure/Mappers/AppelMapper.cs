using Presences.Domain.Entities;
using Presences.Infrastructure.Dtos;

namespace Presences.Infrastructure.Mappers;

public static class AppelMapper
{
    public static Appel ToEntity(AppelDto dto)
    {
        return new Appel
        {
            Id = dto.Id,
            ClasseId = dto.ClasseId,
            EnseignantId = dto.EnseignantId,
            SeanceId = dto.SeanceId,
            DateAppel = dto.DateAppel,
            HeureAppel = dto.HeureAppel,
            EstCloture = dto.EstCloture,
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

    public static AppelDto ToDto(Appel entity)
    {
        return new AppelDto
        {
            Id = entity.Id,
            ClasseId = entity.ClasseId,
            EnseignantId = entity.EnseignantId,
            SeanceId = entity.SeanceId,
            DateAppel = entity.DateAppel,
            HeureAppel = entity.HeureAppel,
            EstCloture = entity.EstCloture,
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
