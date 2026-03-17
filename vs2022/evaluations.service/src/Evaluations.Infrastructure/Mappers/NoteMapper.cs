using Evaluations.Domain.Entities;
using Evaluations.Infrastructure.Dtos;

namespace Evaluations.Infrastructure.Mappers;

public static class NoteMapper
{
    public static Note ToEntity(NoteDto dto)
    {
        return new Note
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            EvaluationId = dto.EvaluationId,
            EleveId = dto.EleveId,
            Valeur = dto.Valeur,
            Commentaire = dto.Commentaire,
            DateSaisie = dto.DateSaisie,
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

    public static NoteDto ToDto(Note entity)
    {
        return new NoteDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            EvaluationId = entity.EvaluationId,
            EleveId = entity.EleveId,
            Valeur = entity.Valeur,
            Commentaire = entity.Commentaire,
            DateSaisie = entity.DateSaisie,
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
