using Presences.Domain.Entities;
using Presences.Infrastructure.Dtos;

namespace Presences.Infrastructure.Mappers;

public static class AbsenceMapper
{
    public static Absence ToEntity(AbsenceDto dto)
    {
        return new Absence
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            EleveId = dto.EleveId,
            AppelId = dto.AppelId,
            DateAbsence = dto.DateAbsence,
            HeureDebut = dto.HeureDebut,
            HeureFin = dto.HeureFin,
            EstJustifiee = dto.EstJustifiee,
            MotifJustification = dto.MotifJustification,
            PieceJointeUrl = dto.PieceJointeUrl,
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

    public static AbsenceDto ToDto(Absence entity)
    {
        return new AbsenceDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            EleveId = entity.EleveId,
            AppelId = entity.AppelId,
            DateAbsence = entity.DateAbsence,
            HeureDebut = entity.HeureDebut,
            HeureFin = entity.HeureFin,
            EstJustifiee = entity.EstJustifiee,
            MotifJustification = entity.MotifJustification,
            PieceJointeUrl = entity.PieceJointeUrl,
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
