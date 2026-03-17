using Dapper;
using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Output;
using Communication.Infrastructure.Dtos;
using Communication.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Communication.Infrastructure.Repositories;

public sealed class AnnonceRepository : IAnnonceRepository
{
    private readonly AuditRepository<AnnonceDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public AnnonceRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<AnnonceDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<AnnonceDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Annonce?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : AnnonceMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Annonce>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(AnnonceMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Annonce>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "name ILIKE @Search OR contenu ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        if (typeId.HasValue)
        {
            where += string.IsNullOrEmpty(where) ? "type_id = @TypeId" : " AND type_id = @TypeId";
            dynamicParams.Add("TypeId", typeId.Value);
        }

        var spec = new SimpleSpecification<AnnonceDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(AnnonceMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Annonce>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Annonce> AddAsync(Annonce entity, CancellationToken ct = default)
    {
        var dto = AnnonceMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return AnnonceMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Annonce entity, CancellationToken ct = default)
    {
        var dto = AnnonceMapper.ToDto(entity);
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
        const string sql = "SELECT id, name, description FROM communication.type_annonces WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }
}
