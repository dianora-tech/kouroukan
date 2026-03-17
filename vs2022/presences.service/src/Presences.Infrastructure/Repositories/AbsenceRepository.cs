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

public sealed class AbsenceRepository : IAbsenceRepository
{
    private readonly AuditRepository<AbsenceDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public AbsenceRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<AbsenceDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<AbsenceDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Absence?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : AbsenceMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Absence>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(AbsenceMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Absence>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "motif_justification ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        if (typeId.HasValue)
        {
            where += string.IsNullOrEmpty(where) ? "type_id = @TypeId" : " AND type_id = @TypeId";
            dynamicParams.Add("TypeId", typeId.Value);
        }

        var spec = new SimpleSpecification<AbsenceDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(AbsenceMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Absence>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Absence> AddAsync(Absence entity, CancellationToken ct = default)
    {
        var dto = AbsenceMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return AbsenceMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Absence entity, CancellationToken ct = default)
    {
        var dto = AbsenceMapper.ToDto(entity);
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
        const string sql = "SELECT id, name, description FROM presences.type_absences WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }
}
