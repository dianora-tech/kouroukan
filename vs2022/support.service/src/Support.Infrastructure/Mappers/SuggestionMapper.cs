using Support.Domain.Entities;
using Support.Infrastructure.Dtos;

namespace Support.Infrastructure.Mappers;

public static class SuggestionMapper
{
    public static Suggestion ToEntity(SuggestionDto dto) => new()
    {
        Id = dto.Id,
        TypeId = dto.TypeId,
        AuteurId = dto.AuteurId,
        Titre = dto.Titre,
        Contenu = dto.Contenu,
        ModuleConcerne = dto.ModuleConcerne,
        StatutSuggestion = dto.StatutSuggestion,
        NombreVotes = dto.NombreVotes,
        CommentaireAdmin = dto.CommentaireAdmin,
        UserId = dto.UserId,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        CreatedBy = dto.CreatedBy,
        UpdatedBy = dto.UpdatedBy,
        IsDeleted = dto.IsDeleted,
        DeletedAt = dto.DeletedAt,
        DeletedBy = dto.DeletedBy
    };

    public static SuggestionDto ToDto(Suggestion entity) => new()
    {
        Id = entity.Id,
        TypeId = entity.TypeId,
        AuteurId = entity.AuteurId,
        Titre = entity.Titre,
        Contenu = entity.Contenu,
        ModuleConcerne = entity.ModuleConcerne,
        StatutSuggestion = entity.StatutSuggestion,
        NombreVotes = entity.NombreVotes,
        CommentaireAdmin = entity.CommentaireAdmin,
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
