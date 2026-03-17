using Support.Domain.Entities;
using Support.Infrastructure.Dtos;

namespace Support.Infrastructure.Mappers;

public static class ReponseTicketMapper
{
    public static ReponseTicket ToEntity(ReponseTicketDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        TicketId = dto.TicketId,
        AuteurId = dto.AuteurId,
        Contenu = dto.Contenu,
        EstReponseIA = dto.EstReponseIA,
        EstInterne = dto.EstInterne,
        UserId = dto.UserId,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        CreatedBy = dto.CreatedBy,
        UpdatedBy = dto.UpdatedBy,
        IsDeleted = dto.IsDeleted,
        DeletedAt = dto.DeletedAt,
        DeletedBy = dto.DeletedBy
    };

    public static ReponseTicketDto ToDto(ReponseTicket entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        TicketId = entity.TicketId,
        AuteurId = entity.AuteurId,
        Contenu = entity.Contenu,
        EstReponseIA = entity.EstReponseIA,
        EstInterne = entity.EstInterne,
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
