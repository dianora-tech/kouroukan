using Communication.Domain.Entities;
using Communication.Infrastructure.Dtos;

namespace Communication.Infrastructure.Mappers;

public static class NotificationMapper
{
    public static Notification ToEntity(NotificationDto dto)
    {
        return new Notification
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            TypeId = dto.TypeId,
            DestinatairesIds = dto.DestinatairesIds,
            Contenu = dto.Contenu,
            Canal = dto.Canal,
            EstEnvoyee = dto.EstEnvoyee,
            DateEnvoi = dto.DateEnvoi,
            LienAction = dto.LienAction,
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

    public static NotificationDto ToDto(Notification entity)
    {
        return new NotificationDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            TypeId = entity.TypeId,
            DestinatairesIds = entity.DestinatairesIds,
            Contenu = entity.Contenu,
            Canal = entity.Canal,
            EstEnvoyee = entity.EstEnvoyee,
            DateEnvoi = entity.DateEnvoi,
            LienAction = entity.LienAction,
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
