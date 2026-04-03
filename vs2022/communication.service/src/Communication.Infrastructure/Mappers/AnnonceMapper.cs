using Communication.Domain.Entities;
using Communication.Infrastructure.Dtos;

namespace Communication.Infrastructure.Mappers;

public static class AnnonceMapper
{
    public static Annonce ToEntity(AnnonceDto dto)
    {
        return new Annonce
        {
            Id = dto.Id,
            Name = dto.Name,
            TypeId = dto.TypeId,
            Contenu = dto.Contenu,
            DateDebut = dto.DateDebut,
            DateFin = dto.DateFin,
            EstActive = dto.EstActive,
            CibleAudience = dto.CibleAudience,
            Priorite = dto.Priorite,
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

    public static AnnonceDto ToDto(Annonce entity)
    {
        return new AnnonceDto
        {
            Id = entity.Id,
            Name = entity.Name,
            TypeId = entity.TypeId,
            Contenu = entity.Contenu,
            DateDebut = entity.DateDebut,
            DateFin = entity.DateFin,
            EstActive = entity.EstActive,
            CibleAudience = entity.CibleAudience,
            Priorite = entity.Priorite,
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
