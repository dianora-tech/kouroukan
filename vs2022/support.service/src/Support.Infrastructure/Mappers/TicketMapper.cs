using Support.Domain.Entities;
using Support.Infrastructure.Dtos;

namespace Support.Infrastructure.Mappers;

public static class TicketMapper
{
    public static Ticket ToEntity(TicketDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        TypeId = dto.TypeId,
        AuteurId = dto.AuteurId,
        Sujet = dto.Sujet,
        Contenu = dto.Contenu,
        Priorite = dto.Priorite,
        StatutTicket = dto.StatutTicket,
        CategorieTicket = dto.CategorieTicket,
        ModuleConcerne = dto.ModuleConcerne,
        AssigneAId = dto.AssigneAId,
        DateResolution = dto.DateResolution,
        NoteSatisfaction = dto.NoteSatisfaction,
        PieceJointeUrl = dto.PieceJointeUrl,
        UserId = dto.UserId,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        CreatedBy = dto.CreatedBy,
        UpdatedBy = dto.UpdatedBy,
        IsDeleted = dto.IsDeleted,
        DeletedAt = dto.DeletedAt,
        DeletedBy = dto.DeletedBy
    };

    public static TicketDto ToDto(Ticket entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        TypeId = entity.TypeId,
        AuteurId = entity.AuteurId,
        Sujet = entity.Sujet,
        Contenu = entity.Contenu,
        Priorite = entity.Priorite,
        StatutTicket = entity.StatutTicket,
        CategorieTicket = entity.CategorieTicket,
        ModuleConcerne = entity.ModuleConcerne,
        AssigneAId = entity.AssigneAId,
        DateResolution = entity.DateResolution,
        NoteSatisfaction = entity.NoteSatisfaction,
        PieceJointeUrl = entity.PieceJointeUrl,
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
