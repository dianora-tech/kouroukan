using ServicesPremium.Domain.Entities;
using ServicesPremium.Infrastructure.Dtos;

namespace ServicesPremium.Infrastructure.Mappers;

/// <summary>
/// Mapper entre Souscription (entite) et SouscriptionDto.
/// </summary>
public static class SouscriptionMapper
{
    public static Souscription ToEntity(SouscriptionDto dto)
    {
        return new Souscription
        {
            Id = dto.Id,
            ServiceParentId = dto.ServiceParentId,
            ParentId = dto.ParentId,
            DateDebut = dto.DateDebut,
            DateFin = dto.DateFin,
            StatutSouscription = dto.StatutSouscription,
            MontantPaye = dto.MontantPaye,
            RenouvellementAuto = dto.RenouvellementAuto,
            DateProchainRenouvellement = dto.DateProchainRenouvellement,
            UserId = dto.UserId,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy
        };
    }

    public static SouscriptionDto ToDto(Souscription entity)
    {
        return new SouscriptionDto
        {
            Id = entity.Id,
            ServiceParentId = entity.ServiceParentId,
            ParentId = entity.ParentId,
            DateDebut = entity.DateDebut,
            DateFin = entity.DateFin,
            StatutSouscription = entity.StatutSouscription,
            MontantPaye = entity.MontantPaye,
            RenouvellementAuto = entity.RenouvellementAuto,
            DateProchainRenouvellement = entity.DateProchainRenouvellement,
            UserId = entity.UserId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.UpdatedBy,
            IsDeleted = entity.IsDeleted,
            DeletedAt = entity.DeletedAt,
            DeletedBy = entity.DeletedBy
        };
    }
}
