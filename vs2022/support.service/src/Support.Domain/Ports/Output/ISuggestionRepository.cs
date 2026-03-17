using GnDapper.Models;
using Support.Domain.Entities;

namespace Support.Domain.Ports.Output;

/// <summary>
/// Repository pour les suggestions.
/// </summary>
public interface ISuggestionRepository
{
    Task<Suggestion?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Suggestion>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Suggestion>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Suggestion> AddAsync(Suggestion entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Suggestion entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<VoteSuggestion?> GetVoteAsync(int suggestionId, int votantId, CancellationToken ct = default);
    Task<VoteSuggestion> AddVoteAsync(VoteSuggestion vote, CancellationToken ct = default);
    Task<bool> DeleteVoteAsync(int suggestionId, int votantId, CancellationToken ct = default);
    Task<bool> IncrementVotesAsync(int suggestionId, CancellationToken ct = default);
    Task<bool> DecrementVotesAsync(int suggestionId, CancellationToken ct = default);
    Task<IReadOnlyList<Suggestion>> GetTopVoteesAsync(int limit = 10, CancellationToken ct = default);
}
