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

public sealed class BadgeageRepository : IBadgeageRepository
{
    private readonly AuditRepository<BadgeageDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public BadgeageRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<BadgeageDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<BadgeageDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Badgeage?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : BadgeageMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Badgeage>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(BadgeageMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Badgeage>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "point_acces ILIKE @Search OR methode_badgeage ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        if (typeId.HasValue)
        {
            where += string.IsNullOrEmpty(where) ? "type_id = @TypeId" : " AND type_id = @TypeId";
            dynamicParams.Add("TypeId", typeId.Value);
        }

        var spec = new SimpleSpecification<BadgeageDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(BadgeageMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Badgeage>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Badgeage> AddAsync(Badgeage entity, CancellationToken ct = default)
    {
        var dto = BadgeageMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return BadgeageMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Badgeage entity, CancellationToken ct = default)
    {
        var dto = BadgeageMapper.ToDto(entity);
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

    public async Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT id, name, description FROM presences.type_badgeages WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }
}
