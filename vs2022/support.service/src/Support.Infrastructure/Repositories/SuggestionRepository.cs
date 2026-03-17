using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Support.Domain.Entities;
using Support.Domain.Ports.Output;
using Support.Infrastructure.Dtos;
using Support.Infrastructure.Mappers;

namespace Support.Infrastructure.Repositories;

/// <summary>
/// Repository pour les suggestions (PostgreSQL).
/// </summary>
public sealed class SuggestionRepository : ISuggestionRepository
{
    private readonly AuditRepository<SuggestionDto> _repo;
    private readonly AuditRepository<VoteSuggestionDto> _voteRepo;

    public SuggestionRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<SuggestionDto>> logger,
        ILogger<Repository<VoteSuggestionDto>> voteLogger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<SuggestionDto>(connectionFactory, logger, options, httpContextAccessor);
        _voteRepo = new AuditRepository<VoteSuggestionDto>(connectionFactory, voteLogger, options, httpContextAccessor);
    }

    public async Task<Suggestion?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct);
        return dto is null ? null : SuggestionMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Suggestion>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct);
        return dtos.Select(SuggestionMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Suggestion>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(titre ILIKE @Search OR contenu ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        if (typeId.HasValue)
        {
            conditions.Add("type_id = @TypeId");
            parameters["TypeId"] = typeId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : null;
        var spec = new SimpleSpecification<SuggestionDto>(
            where,
            parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize,
            pageSize);

        var result = await _repo.FindPagedAsync(spec, ct);
        var entities = result.Items.Select(SuggestionMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Suggestion>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Suggestion> AddAsync(Suggestion entity, CancellationToken ct = default)
    {
        var dto = SuggestionMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct);
        return SuggestionMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Suggestion entity, CancellationToken ct = default)
    {
        var dto = SuggestionMapper.ToDto(entity);
        return await _repo.UpdateAsync(dto, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        await _repo.DeleteAsync(id, ct);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
        await _repo.ExistsAsync(id, ct);

    public async Task<VoteSuggestion?> GetVoteAsync(int suggestionId, int votantId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT * FROM support.votes_suggestions
            WHERE suggestion_id = @SuggestionId AND votant_id = @VotantId AND is_deleted = FALSE";
        var dtos = await _voteRepo.GetWithQueryAsync(sql, new { SuggestionId = suggestionId, VotantId = votantId }, ct);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : VoteSuggestionMapper.ToEntity(dto);
    }

    public async Task<VoteSuggestion> AddVoteAsync(VoteSuggestion vote, CancellationToken ct = default)
    {
        var dto = VoteSuggestionMapper.ToDto(vote);
        var created = await _voteRepo.AddAsync(dto, ct);
        return VoteSuggestionMapper.ToEntity(created);
    }

    public async Task<bool> DeleteVoteAsync(int suggestionId, int votantId, CancellationToken ct = default)
    {
        var vote = await GetVoteAsync(suggestionId, votantId, ct);
        if (vote is null) return false;
        return await _voteRepo.DeleteAsync(vote.Id, ct);
    }

    public async Task<bool> IncrementVotesAsync(int suggestionId, CancellationToken ct = default)
    {
        var suggestion = await GetByIdAsync(suggestionId, ct);
        if (suggestion is null) return false;
        suggestion.NombreVotes++;
        return await UpdateAsync(suggestion, ct);
    }

    public async Task<bool> DecrementVotesAsync(int suggestionId, CancellationToken ct = default)
    {
        var suggestion = await GetByIdAsync(suggestionId, ct);
        if (suggestion is null) return false;
        suggestion.NombreVotes = Math.Max(0, suggestion.NombreVotes - 1);
        return await UpdateAsync(suggestion, ct);
    }

    public async Task<IReadOnlyList<Suggestion>> GetTopVoteesAsync(int limit = 10, CancellationToken ct = default)
    {
        var sql = $"SELECT * FROM support.suggestions WHERE is_deleted = FALSE ORDER BY nombre_votes DESC LIMIT @Limit";
        var dtos = await _repo.GetWithQueryAsync(sql, new { Limit = limit }, ct);
        return dtos.Select(SuggestionMapper.ToEntity).ToList().AsReadOnly();
    }
}
