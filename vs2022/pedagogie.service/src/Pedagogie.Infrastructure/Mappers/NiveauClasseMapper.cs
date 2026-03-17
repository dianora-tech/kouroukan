using Pedagogie.Domain.Entities;
using Pedagogie.Infrastructure.Dtos;

namespace Pedagogie.Infrastructure.Mappers;

public static class NiveauClasseMapper
{
    public static NiveauClasse ToEntity(NiveauClasseDto dto)
    {
        return new NiveauClasse
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            TypeId = dto.TypeId,
            Code = dto.Code,
            Ordre = dto.Ordre,
            CycleEtude = dto.CycleEtude,
            AgeOfficielEntree = dto.AgeOfficielEntree,
            MinistereTutelle = dto.MinistereTutelle,
            ExamenSortie = dto.ExamenSortie,
            TauxHoraireEnseignant = dto.TauxHoraireEnseignant,
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

    public static NiveauClasseDto ToDto(NiveauClasse entity)
    {
        return new NiveauClasseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            TypeId = entity.TypeId,
            Code = entity.Code,
            Ordre = entity.Ordre,
            CycleEtude = entity.CycleEtude,
            AgeOfficielEntree = entity.AgeOfficielEntree,
            MinistereTutelle = entity.MinistereTutelle,
            ExamenSortie = entity.ExamenSortie,
            TauxHoraireEnseignant = entity.TauxHoraireEnseignant,
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
