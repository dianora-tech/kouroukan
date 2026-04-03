using Personnel.Domain.Entities;
using Personnel.Infrastructure.Dtos;

namespace Personnel.Infrastructure.Mappers;

public static class EnseignantMapper
{
    public static Enseignant ToEntity(EnseignantDto dto)
    {
        return new Enseignant
        {
            Id = dto.Id,
            Matricule = dto.Matricule,
            Specialite = dto.Specialite,
            DateEmbauche = dto.DateEmbauche,
            ModeRemuneration = dto.ModeRemuneration,
            MontantForfait = dto.MontantForfait,
            Telephone = dto.Telephone,
            Email = dto.Email,
            StatutEnseignant = dto.StatutEnseignant,
            SoldeCongesAnnuel = dto.SoldeCongesAnnuel,
            TypeId = dto.TypeId,
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

    public static EnseignantDto ToDto(Enseignant entity)
    {
        return new EnseignantDto
        {
            Id = entity.Id,
            Matricule = entity.Matricule,
            Specialite = entity.Specialite,
            DateEmbauche = entity.DateEmbauche,
            ModeRemuneration = entity.ModeRemuneration,
            MontantForfait = entity.MontantForfait,
            Telephone = entity.Telephone,
            Email = entity.Email,
            StatutEnseignant = entity.StatutEnseignant,
            SoldeCongesAnnuel = entity.SoldeCongesAnnuel,
            TypeId = entity.TypeId,
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
