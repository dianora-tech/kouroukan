using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Output;
using Pedagogie.Infrastructure.Dtos;
using Pedagogie.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pedagogie.Infrastructure.Repositories;

public sealed class SeanceRepository : ISeanceRepository
{
    private readonly AuditRepository<SeanceDto> _repo;

    public SeanceRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<SeanceDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<SeanceDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Seance?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : SeanceMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Seance>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(SeanceMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Seance>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "name ILIKE @Search";
            parameters = new { Search = $"%{search}%" };
        }

        var spec = new SimpleSpecification<SeanceDto>(
            where, parameters,
            string.IsNullOrWhiteSpace(orderBy) ? "jour_semaine ASC, heure_debut ASC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(SeanceMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Seance>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Seance> AddAsync(Seance entity, CancellationToken ct = default)
    {
        var dto = SeanceMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return SeanceMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Seance entity, CancellationToken ct = default)
    {
        var dto = SeanceMapper.ToDto(entity);
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
}
