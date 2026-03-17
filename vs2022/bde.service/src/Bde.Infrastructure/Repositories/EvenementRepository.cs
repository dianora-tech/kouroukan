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

public sealed class EvenementRepository : IEvenementRepository
{
    private readonly AuditRepository<EvenementDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public EvenementRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<EvenementDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<EvenementDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Evenement?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : EvenementMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Evenement>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(EvenementMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Evenement>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "name ILIKE @Search OR lieu ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        if (typeId.HasValue)
        {
            where += string.IsNullOrEmpty(where) ? "type_id = @TypeId" : " AND type_id = @TypeId";
            dynamicParams.Add("TypeId", typeId.Value);
        }

        var spec = new SimpleSpecification<EvenementDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "date_evenement DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(EvenementMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Evenement>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Evenement> AddAsync(Evenement entity, CancellationToken ct = default)
    {
        var dto = EvenementMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return EvenementMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Evenement entity, CancellationToken ct = default)
    {
        var dto = EvenementMapper.ToDto(entity);
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
        const string sql = "SELECT id, name, description FROM bde.type_evenements WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }
}
