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

public sealed class MatiereRepository : IMatiereRepository
{
    private readonly AuditRepository<MatiereDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public MatiereRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<MatiereDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<MatiereDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Matiere?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : MatiereMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Matiere>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(MatiereMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Matiere>> GetPagedAsync(
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

        var spec = new SimpleSpecification<MatiereDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "code ASC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(MatiereMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Matiere>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Matiere> AddAsync(Matiere entity, CancellationToken ct = default)
    {
        var dto = MatiereMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return MatiereMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Matiere entity, CancellationToken ct = default)
    {
        var dto = MatiereMapper.ToDto(entity);
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
        const string sql = "SELECT id, name, description FROM pedagogie.type_matieres WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }

    public async Task<TypeDto?> GetTypeByIdAsync(int id, CancellationToken ct = default)
    {
        const string sql = "SELECT id, name, description FROM pedagogie.type_matieres WHERE id = @Id AND is_deleted = FALSE";
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<TypeDto>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct)).ConfigureAwait(false);
    }

    public async Task<TypeDto> AddTypeAsync(string name, string? description, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO pedagogie.type_matieres (name, description)
            VALUES (@Name, @Description)
            RETURNING id, name, description
            """;
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<TypeDto>(
            new CommandDefinition(sql, new { Name = name, Description = description }, cancellationToken: ct)).ConfigureAwait(false);
    }

    public async Task<bool> UpdateTypeAsync(int id, string name, string? description, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE pedagogie.type_matieres
            SET name = @Name, description = @Description, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """;
        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id, Name = name, Description = description }, cancellationToken: ct)).ConfigureAwait(false);
        return affected > 0;
    }

    public async Task<bool> DeleteTypeAsync(int id, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE pedagogie.type_matieres
            SET is_deleted = TRUE, deleted_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """;
        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct)).ConfigureAwait(false);
        return affected > 0;
    }
}
