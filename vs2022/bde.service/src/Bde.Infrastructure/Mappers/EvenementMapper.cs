using Bde.Domain.Entities;
using Bde.Infrastructure.Dtos;

namespace Bde.Infrastructure.Mappers;

public static class EvenementMapper
{
    public static Evenement ToEntity(EvenementDto dto)
    {
        return new Evenement
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            Name = dto.Name,
            Description = dto.Description,
            AssociationId = dto.AssociationId,
            DateEvenement = dto.DateEvenement,
            Lieu = dto.Lieu,
            Capacite = dto.Capacite,
            TarifEntree = dto.TarifEntree,
            NombreInscrits = dto.NombreInscrits,
            StatutEvenement = dto.StatutEvenement,
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

    public static EvenementDto ToDto(Evenement entity)
    {
        return new EvenementDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            Name = entity.Name,
            Description = entity.Description,
            AssociationId = entity.AssociationId,
            DateEvenement = entity.DateEvenement,
            Lieu = entity.Lieu,
            Capacite = entity.Capacite,
            TarifEntree = entity.TarifEntree,
            NombreInscrits = entity.NombreInscrits,
            StatutEvenement = entity.StatutEvenement,
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
