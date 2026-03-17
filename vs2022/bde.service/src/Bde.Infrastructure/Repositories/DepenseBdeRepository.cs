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

public sealed class DepenseBdeRepository : IDepenseBdeRepository
{
    private readonly AuditRepository<DepenseBdeDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public DepenseBdeRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<DepenseBdeDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<DepenseBdeDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<DepenseBde?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : DepenseBdeMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<DepenseBde>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(DepenseBdeMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<DepenseBde>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "motif ILIKE @Search OR categorie ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        if (typeId.HasValue)
        {
            where += string.IsNullOrEmpty(where) ? "type_id = @TypeId" : " AND type_id = @TypeId";
            dynamicParams.Add("TypeId", typeId.Value);
        }

        var spec = new SimpleSpecification<DepenseBdeDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(DepenseBdeMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<DepenseBde>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<DepenseBde> AddAsync(DepenseBde entity, CancellationToken ct = default)
    {
        var dto = DepenseBdeMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return DepenseBdeMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(DepenseBde entity, CancellationToken ct = default)
    {
        var dto = DepenseBdeMapper.ToDto(entity);
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
        const string sql = "SELECT id, name, description FROM bde.type_depenses_bde WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }
}
