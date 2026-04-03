using Evaluations.Domain.Entities;
using Evaluations.Infrastructure.Dtos;

namespace Evaluations.Infrastructure.Mappers;

public static class EvaluationMapper
{
    public static Evaluation ToEntity(EvaluationDto dto)
    {
        return new Evaluation
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            MatiereId = dto.MatiereId,
            ClasseId = dto.ClasseId,
            EnseignantId = dto.EnseignantId,
            DateEvaluation = dto.DateEvaluation,
            Coefficient = dto.Coefficient,
            NoteMaximale = dto.NoteMaximale,
            Trimestre = dto.Trimestre,
            AnneeScolaireId = dto.AnneeScolaireId,
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

    public static EvaluationDto ToDto(Evaluation entity)
    {
        return new EvaluationDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            MatiereId = entity.MatiereId,
            ClasseId = entity.ClasseId,
            EnseignantId = entity.EnseignantId,
            DateEvaluation = entity.DateEvaluation,
            Coefficient = entity.Coefficient,
            NoteMaximale = entity.NoteMaximale,
            Trimestre = entity.Trimestre,
            AnneeScolaireId = entity.AnneeScolaireId,
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
