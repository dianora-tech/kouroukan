using Support.Domain.Entities;
using Support.Infrastructure.Dtos;

namespace Support.Infrastructure.Mappers;

public static class ConversationIAMapper
{
    public static ConversationIA ToEntity(ConversationIADto dto) => new()
    {
        Id = dto.Id,
        UtilisateurId = dto.UtilisateurId,
        Titre = dto.Titre,
        EstActive = dto.EstActive,
        NombreMessages = dto.NombreMessages,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        CreatedBy = dto.CreatedBy,
        UpdatedBy = dto.UpdatedBy,
        IsDeleted = dto.IsDeleted,
        DeletedAt = dto.DeletedAt,
        DeletedBy = dto.DeletedBy
    };

    public static ConversationIADto ToDto(ConversationIA entity) => new()
    {
        Id = entity.Id,
        UtilisateurId = entity.UtilisateurId,
        Titre = entity.Titre,
        EstActive = entity.EstActive,
        NombreMessages = entity.NombreMessages,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedBy = entity.UpdatedBy,
        IsDeleted = entity.IsDeleted,
        DeletedAt = entity.DeletedAt,
        DeletedBy = entity.DeletedBy
    };
}
