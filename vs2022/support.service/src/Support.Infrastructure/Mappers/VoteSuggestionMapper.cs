using Support.Domain.Entities;
using Support.Infrastructure.Dtos;

namespace Support.Infrastructure.Mappers;

public static class VoteSuggestionMapper
{
    public static VoteSuggestion ToEntity(VoteSuggestionDto dto) => new()
    {
        Id = dto.Id,
        SuggestionId = dto.SuggestionId,
        VotantId = dto.VotantId,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        CreatedBy = dto.CreatedBy,
        UpdatedBy = dto.UpdatedBy,
        IsDeleted = dto.IsDeleted,
        DeletedAt = dto.DeletedAt,
        DeletedBy = dto.DeletedBy
    };

    public static VoteSuggestionDto ToDto(VoteSuggestion entity) => new()
    {
        Id = entity.Id,
        SuggestionId = entity.SuggestionId,
        VotantId = entity.VotantId,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedBy = entity.UpdatedBy,
        IsDeleted = entity.IsDeleted,
        DeletedAt = entity.DeletedAt,
        DeletedBy = entity.DeletedBy
    };
}
