using Dapper;
using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Output;
using Bde.Infrastructure.Dtos;
using Bde.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bde.Infrastructure.Repositories;

public sealed class MembreBdeRepository : IMembreBdeRepository
{
    private readonly AuditRepository<MembreBdeDto> _repo;

    public MembreBdeRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<MembreBdeDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<MembreBdeDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<MembreBde?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : MembreBdeMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<MembreBde>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(MembreBdeMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<MembreBde>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "name ILIKE @Search OR role_bde ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        var spec = new SimpleSpecification<MembreBdeDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(MembreBdeMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<MembreBde>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<MembreBde> AddAsync(MembreBde entity, CancellationToken ct = default)
    {
        var dto = MembreBdeMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return MembreBdeMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(MembreBde entity, CancellationToken ct = default)
    {
        var dto = MembreBdeMapper.ToDto(entity);
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
