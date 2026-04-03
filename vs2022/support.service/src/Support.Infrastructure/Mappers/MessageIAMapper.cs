using Support.Domain.Entities;
using Support.Infrastructure.Dtos;

namespace Support.Infrastructure.Mappers;

public static class MessageIAMapper
{
    public static MessageIA ToEntity(MessageIADto dto) => new()
    {
        Id = dto.Id,
        ConversationIAId = dto.ConversationIAId,
        Role = dto.Role,
        Contenu = dto.Contenu,
        ContexteArticlesIds = dto.ContexteArticlesIds,
        TokensUtilises = dto.TokensUtilises,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        CreatedBy = dto.CreatedBy,
        UpdatedBy = dto.UpdatedBy,
        IsDeleted = dto.IsDeleted,
        DeletedAt = dto.DeletedAt,
        DeletedBy = dto.DeletedBy
    };

    public static MessageIADto ToDto(MessageIA entity) => new()
    {
        Id = entity.Id,
        ConversationIAId = entity.ConversationIAId,
        Role = entity.Role,
        Contenu = entity.Contenu,
        ContexteArticlesIds = entity.ContexteArticlesIds,
        TokensUtilises = entity.TokensUtilises,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedBy = entity.UpdatedBy,
        IsDeleted = entity.IsDeleted,
        DeletedAt = entity.DeletedAt,
        DeletedBy = entity.DeletedBy
    };
}
