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
            MatiereId = dto.MatiereId,
            ClasseId = dto.ClasseId,
            EnseignantId = dto.EnseignantId,
            SalleId = dto.SalleId,
            JourSemaine = dto.JourSemaine,
            HeureDebut = dto.HeureDebut,
            HeureFin = dto.HeureFin,
            AnneeScolaireId = dto.AnneeScolaireId,
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
            MatiereId = entity.MatiereId,
            ClasseId = entity.ClasseId,
            EnseignantId = entity.EnseignantId,
            SalleId = entity.SalleId,
            JourSemaine = entity.JourSemaine,
            HeureDebut = entity.HeureDebut,
            HeureFin = entity.HeureFin,
            AnneeScolaireId = entity.AnneeScolaireId,
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
