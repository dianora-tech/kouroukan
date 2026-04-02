using Pedagogie.Domain.Entities;
using Pedagogie.Infrastructure.Dtos;

namespace Pedagogie.Infrastructure.Mappers;

public static class AffectationEnseignantMapper
{
    public static AffectationEnseignant ToEntity(AffectationEnseignantDto dto)
    {
        return new AffectationEnseignant
        {
            Id = dto.Id,
            LiaisonId = dto.LiaisonId,
            ClasseId = dto.ClasseId,
            MatiereId = dto.MatiereId,
            AnneeScolaireId = dto.AnneeScolaireId,
            EstActive = dto.EstActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static AffectationEnseignantDto ToDto(AffectationEnseignant entity)
    {
        return new AffectationEnseignantDto
        {
            Id = entity.Id,
            LiaisonId = entity.LiaisonId,
            ClasseId = entity.ClasseId,
            MatiereId = entity.MatiereId,
            AnneeScolaireId = entity.AnneeScolaireId,
            EstActive = entity.EstActive,
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
