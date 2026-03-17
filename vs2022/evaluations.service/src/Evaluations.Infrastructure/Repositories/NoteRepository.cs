using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Output;
using Evaluations.Infrastructure.Dtos;
using Evaluations.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Evaluations.Infrastructure.Repositories;

public sealed class NoteRepository : INoteRepository
{
    private readonly AuditRepository<NoteDto> _repo;

    public NoteRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<NoteDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<NoteDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Note?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : NoteMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(NoteMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Note>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "commentaire ILIKE @Search OR name ILIKE @Search";
            parameters = new { Search = $"%{search}%" };
        }

        var spec = new SimpleSpecification<NoteDto>(
            where, parameters,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(NoteMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Note>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Note> AddAsync(Note entity, CancellationToken ct = default)
    {
        var dto = NoteMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return NoteMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Note entity, CancellationToken ct = default)
    {
        var dto = NoteMapper.ToDto(entity);
        return await _repo.UpdateAsync(dto, ct).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _repo.ExistsAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Note>> GetByEvaluationIdAsync(int evaluationId, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM evaluations.notes WHERE evaluation_id = @EvaluationId AND is_deleted = FALSE ORDER BY eleve_id";
        var dtos = await _repo.GetWithQueryAsync(sql, new { EvaluationId = evaluationId }, ct).ConfigureAwait(false);
        return dtos.Select(NoteMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Note>> GetByEleveIdAsync(int eleveId, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM evaluations.notes WHERE eleve_id = @EleveId AND is_deleted = FALSE ORDER BY date_saisie DESC";
        var dtos = await _repo.GetWithQueryAsync(sql, new { EleveId = eleveId }, ct).ConfigureAwait(false);
        return dtos.Select(NoteMapper.ToEntity).ToList().AsReadOnly();
    }
}
