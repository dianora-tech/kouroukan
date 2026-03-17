using Dapper;
using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Output;
using Evaluations.Infrastructure.Dtos;
using Evaluations.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Evaluations.Infrastructure.Repositories;

public sealed class EvaluationRepository : IEvaluationRepository
{
    private readonly AuditRepository<EvaluationDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public EvaluationRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<EvaluationDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<EvaluationDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Evaluation?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : EvaluationMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Evaluation>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(EvaluationMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Evaluation>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "name ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        if (typeId.HasValue)
        {
            where += string.IsNullOrEmpty(where) ? "type_id = @TypeId" : " AND type_id = @TypeId";
            dynamicParams.Add("TypeId", typeId.Value);
        }

        var spec = new SimpleSpecification<EvaluationDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(EvaluationMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Evaluation>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Evaluation> AddAsync(Evaluation entity, CancellationToken ct = default)
    {
        var dto = EvaluationMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return EvaluationMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Evaluation entity, CancellationToken ct = default)
    {
        var dto = EvaluationMapper.ToDto(entity);
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
        const string sql = "SELECT id, name, description FROM evaluations.type_evaluations WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }
}
