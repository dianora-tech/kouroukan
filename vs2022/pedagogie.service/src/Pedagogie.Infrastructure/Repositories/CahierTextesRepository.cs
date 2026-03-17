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

public sealed class CahierTextesRepository : ICahierTextesRepository
{
    private readonly AuditRepository<CahierTextesDto> _repo;

    public CahierTextesRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<CahierTextesDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<CahierTextesDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<CahierTextes?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : CahierTextesMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<CahierTextes>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(CahierTextesMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<CahierTextes>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "(name ILIKE @Search OR contenu ILIKE @Search)";
            parameters = new { Search = $"%{search}%" };
        }

        var spec = new SimpleSpecification<CahierTextesDto>(
            where, parameters,
            string.IsNullOrWhiteSpace(orderBy) ? "date_seance DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(CahierTextesMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<CahierTextes>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<CahierTextes> AddAsync(CahierTextes entity, CancellationToken ct = default)
    {
        var dto = CahierTextesMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return CahierTextesMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(CahierTextes entity, CancellationToken ct = default)
    {
        var dto = CahierTextesMapper.ToDto(entity);
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
