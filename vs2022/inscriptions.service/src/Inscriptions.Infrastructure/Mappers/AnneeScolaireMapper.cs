using Inscriptions.Domain.Entities;
using Inscriptions.Infrastructure.Dtos;

namespace Inscriptions.Infrastructure.Mappers;

public static class AnneeScolaireMapper
{
    public static AnneeScolaire ToEntity(AnneeScolaireDto dto)
    {
        return new AnneeScolaire
        {
            Id = dto.Id,
            Libelle = dto.Libelle,
            DateDebut = dto.DateDebut,
            DateFin = dto.DateFin,
            EstActive = dto.EstActive,
            Code = dto.Code,
            Description = dto.Description,
            Statut = dto.Statut,
            DateRentree = dto.DateRentree,
            NombrePeriodes = dto.NombrePeriodes,
            TypePeriode = dto.TypePeriode,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static AnneeScolaireDto ToDto(AnneeScolaire entity)
    {
        return new AnneeScolaireDto
        {
            Id = entity.Id,
            Libelle = entity.Libelle,
            DateDebut = entity.DateDebut,
            DateFin = entity.DateFin,
            EstActive = entity.EstActive,
            Code = entity.Code,
            Description = entity.Description,
            Statut = entity.Statut,
            DateRentree = entity.DateRentree,
            NombrePeriodes = entity.NombrePeriodes,
            TypePeriode = entity.TypePeriode,
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
