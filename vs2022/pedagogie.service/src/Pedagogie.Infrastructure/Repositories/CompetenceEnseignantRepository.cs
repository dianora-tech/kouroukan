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

public sealed class CompetenceEnseignantRepository : ICompetenceEnseignantRepository
{
    private readonly AuditRepository<CompetenceEnseignantDto> _repo;

    public CompetenceEnseignantRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<CompetenceEnseignantDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<CompetenceEnseignantDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<CompetenceEnseignant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : CompetenceEnseignantMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<CompetenceEnseignant>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(CompetenceEnseignantMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<CompetenceEnseignant>> GetPagedAsync(
        int page, int pageSize, int? userId, string? cycleEtude, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (userId.HasValue)
        {
            conditions.Add("user_id = @UserId");
            parameters["UserId"] = userId.Value;
        }

        if (!string.IsNullOrWhiteSpace(cycleEtude))
        {
            conditions.Add("cycle_etude ILIKE @CycleEtude");
            parameters["CycleEtude"] = $"%{cycleEtude}%";
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<CompetenceEnseignantDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(CompetenceEnseignantMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<CompetenceEnseignant>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<CompetenceEnseignant> AddAsync(CompetenceEnseignant entity, CancellationToken ct = default)
    {
        var dto = CompetenceEnseignantMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return CompetenceEnseignantMapper.ToEntity(created);
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
