using Dapper;
using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Output;
using Presences.Infrastructure.Dtos;
using Presences.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Presences.Infrastructure.Repositories;

public sealed class AppelRepository : IAppelRepository
{
    private readonly AuditRepository<AppelDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public AppelRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<AppelDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<AppelDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Appel?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : AppelMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Appel>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(AppelMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Appel>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "CAST(classe_id AS TEXT) ILIKE @Search OR CAST(enseignant_id AS TEXT) ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        var spec = new SimpleSpecification<AppelDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(AppelMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Appel>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Appel> AddAsync(Appel entity, CancellationToken ct = default)
    {
        var dto = AppelMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return AppelMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Appel entity, CancellationToken ct = default)
    {
        var dto = AppelMapper.ToDto(entity);
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
