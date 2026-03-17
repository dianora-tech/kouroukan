using Dapper;
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

public sealed class NiveauClasseRepository : INiveauClasseRepository
{
    private readonly AuditRepository<NiveauClasseDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public NiveauClasseRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<NiveauClasseDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<NiveauClasseDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<NiveauClasse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : NiveauClasseMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<NiveauClasse>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(NiveauClasseMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<NiveauClasse>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "(name ILIKE @Search OR code ILIKE @Search)";
            dynamicParams.Add("Search", $"%{search}%");
        }

        if (typeId.HasValue)
        {
            where += string.IsNullOrEmpty(where) ? "type_id = @TypeId" : " AND type_id = @TypeId";
            dynamicParams.Add("TypeId", typeId.Value);
        }

        var spec = new SimpleSpecification<NiveauClasseDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "ordre ASC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(NiveauClasseMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<NiveauClasse>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<NiveauClasse> AddAsync(NiveauClasse entity, CancellationToken ct = default)
    {
        var dto = NiveauClasseMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return NiveauClasseMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(NiveauClasse entity, CancellationToken ct = default)
    {
        var dto = NiveauClasseMapper.ToDto(entity);
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
        const string sql = "SELECT id, name, description FROM pedagogie.type_niveaux_classes WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }
}
