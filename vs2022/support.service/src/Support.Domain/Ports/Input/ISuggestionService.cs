using GnDapper.Models;
using Support.Domain.Entities;

namespace Support.Domain.Ports.Input;

/// <summary>
/// Service metier pour la gestion des suggestions.
/// </summary>
public interface ISuggestionService
{
    Task<Suggestion?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Suggestion>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Suggestion>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Suggestion> CreateAsync(Suggestion entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Suggestion entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<bool> VoterAsync(int suggestionId, int votantId, int userId, CancellationToken ct = default);
    Task<bool> RetirerVoteAsync(int suggestionId, int votantId, CancellationToken ct = default);
}
