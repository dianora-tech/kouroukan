using Communication.Domain.Entities;
using Communication.Infrastructure.Dtos;

namespace Communication.Infrastructure.Mappers;

public static class MessageMapper
{
    public static Message ToEntity(MessageDto dto)
    {
        return new Message
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            ExpediteurId = dto.ExpediteurId,
            DestinataireId = dto.DestinataireId,
            Sujet = dto.Sujet,
            Contenu = dto.Contenu,
            EstLu = dto.EstLu,
            DateLecture = dto.DateLecture,
            GroupeDestinataire = dto.GroupeDestinataire,
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

    public static MessageDto ToDto(Message entity)
    {
        return new MessageDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            ExpediteurId = entity.ExpediteurId,
            DestinataireId = entity.DestinataireId,
            Sujet = entity.Sujet,
            Contenu = entity.Contenu,
            EstLu = entity.EstLu,
            DateLecture = entity.DateLecture,
            GroupeDestinataire = entity.GroupeDestinataire,
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
