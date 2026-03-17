using ServicesPremium.Domain.Entities;
using ServicesPremium.Infrastructure.Dtos;

namespace ServicesPremium.Infrastructure.Mappers;

/// <summary>
/// Mapper entre ServiceParent (entite) et ServiceParentDto.
/// </summary>
public static class ServiceParentMapper
{
    public static ServiceParent ToEntity(ServiceParentDto dto)
    {
        return new ServiceParent
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            TypeId = dto.TypeId,
            Code = dto.Code,
            Tarif = dto.Tarif,
            Periodicite = dto.Periodicite,
            EstActif = dto.EstActif,
            PeriodeEssaiJours = dto.PeriodeEssaiJours,
            TarifDegressif = dto.TarifDegressif,
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

    public static ServiceParentDto ToDto(ServiceParent entity)
    {
        return new ServiceParentDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            TypeId = entity.TypeId,
            Code = entity.Code,
            Tarif = entity.Tarif,
            Periodicite = entity.Periodicite,
            EstActif = entity.EstActif,
            PeriodeEssaiJours = entity.PeriodeEssaiJours,
            TarifDegressif = entity.TarifDegressif,
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
