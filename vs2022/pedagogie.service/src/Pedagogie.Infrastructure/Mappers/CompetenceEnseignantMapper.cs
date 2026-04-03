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
            IsDeleted = dto.IsDeleted,
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
            IsDeleted = entity.IsDeleted,
        };
    }
}
