using Pedagogie.Domain.Entities;
using Pedagogie.Infrastructure.Dtos;

namespace Pedagogie.Infrastructure.Mappers;

public static class SeanceMapper
{
    public static Seance ToEntity(SeanceDto dto)
    {
        return new Seance
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            MatiereId = dto.MatiereId,
            ClasseId = dto.ClasseId,
            EnseignantId = dto.EnseignantId,
            SalleId = dto.SalleId,
            JourSemaine = int.TryParse(dto.JourSemaine, out var jour) ? jour : 0,
            HeureDebut = dto.HeureDebut,
            HeureFin = dto.HeureFin,
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

    public static SeanceDto ToDto(Seance entity)
    {
        return new SeanceDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            MatiereId = entity.MatiereId,
            ClasseId = entity.ClasseId,
            EnseignantId = entity.EnseignantId,
            SalleId = entity.SalleId,
            JourSemaine = entity.JourSemaine.ToString(),
            HeureDebut = entity.HeureDebut,
            HeureFin = entity.HeureFin,
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
