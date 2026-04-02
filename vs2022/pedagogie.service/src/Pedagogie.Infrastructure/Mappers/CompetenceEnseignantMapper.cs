using Pedagogie.Domain.Entities;
using Pedagogie.Infrastructure.Dtos;

namespace Pedagogie.Infrastructure.Mappers;

public static class CompetenceEnseignantMapper
{
    public static CompetenceEnseignant ToEntity(CompetenceEnseignantDto dto)
    {
        return new CompetenceEnseignant
        {
            Id = dto.Id,
            UserId = dto.UserId,
            MatiereId = dto.MatiereId,
            CycleEtude = dto.CycleEtude,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static CompetenceEnseignantDto ToDto(CompetenceEnseignant entity)
    {
        return new CompetenceEnseignantDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            MatiereId = entity.MatiereId,
            CycleEtude = entity.CycleEtude,
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
