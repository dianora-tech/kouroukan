using Presences.Domain.Entities;
using Presences.Infrastructure.Dtos;

namespace Presences.Infrastructure.Mappers;

public static class BadgeageMapper
{
    public static Badgeage ToEntity(BadgeageDto dto)
    {
        return new Badgeage
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            EleveId = dto.EleveId,
            DateBadgeage = dto.DateBadgeage,
            HeureBadgeage = dto.HeureBadgeage,
            PointAcces = dto.PointAcces,
            MethodeBadgeage = dto.MethodeBadgeage,
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

    public static BadgeageDto ToDto(Badgeage entity)
    {
        return new BadgeageDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            EleveId = entity.EleveId,
            DateBadgeage = entity.DateBadgeage,
            HeureBadgeage = entity.HeureBadgeage,
            PointAcces = entity.PointAcces,
            MethodeBadgeage = entity.MethodeBadgeage,
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
