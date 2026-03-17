using Bde.Domain.Entities;
using Bde.Infrastructure.Dtos;

namespace Bde.Infrastructure.Mappers;

public static class MembreBdeMapper
{
    public static MembreBde ToEntity(MembreBdeDto dto)
    {
        return new MembreBde
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            AssociationId = dto.AssociationId,
            EleveId = dto.EleveId,
            RoleBde = dto.RoleBde,
            DateAdhesion = dto.DateAdhesion,
            MontantCotisation = dto.MontantCotisation,
            EstActif = dto.EstActif,
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

    public static MembreBdeDto ToDto(MembreBde entity)
    {
        return new MembreBdeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            AssociationId = entity.AssociationId,
            EleveId = entity.EleveId,
            RoleBde = entity.RoleBde,
            DateAdhesion = entity.DateAdhesion,
            MontantCotisation = entity.MontantCotisation,
            EstActif = entity.EstActif,
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
