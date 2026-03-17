using Pedagogie.Domain.Entities;
using Pedagogie.Infrastructure.Dtos;

namespace Pedagogie.Infrastructure.Mappers;

public static class SalleMapper
{
    public static Salle ToEntity(SalleDto dto)
    {
        return new Salle
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            TypeId = dto.TypeId,
            Capacite = dto.Capacite,
            Batiment = dto.Batiment,
            Equipements = dto.Equipements,
            EstDisponible = dto.EstDisponible,
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

    public static SalleDto ToDto(Salle entity)
    {
        return new SalleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            TypeId = entity.TypeId,
            Capacite = entity.Capacite,
            Batiment = entity.Batiment,
            Equipements = entity.Equipements,
            EstDisponible = entity.EstDisponible,
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
