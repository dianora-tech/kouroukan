using Pedagogie.Domain.Entities;
using Pedagogie.Infrastructure.Dtos;

namespace Pedagogie.Infrastructure.Mappers;

public static class ClasseMapper
{
    public static Classe ToEntity(ClasseDto dto)
    {
        return new Classe
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            NiveauClasseId = dto.NiveauClasseId,
            Capacite = dto.Capacite,
            AnneeScolaireId = dto.AnneeScolaireId,
            EnseignantPrincipalId = dto.EnseignantPrincipalId,
            Effectif = dto.Effectif,
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

    public static ClasseDto ToDto(Classe entity)
    {
        return new ClasseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            NiveauClasseId = entity.NiveauClasseId,
            Capacite = entity.Capacite,
            AnneeScolaireId = entity.AnneeScolaireId,
            EnseignantPrincipalId = entity.EnseignantPrincipalId,
            Effectif = entity.Effectif,
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
