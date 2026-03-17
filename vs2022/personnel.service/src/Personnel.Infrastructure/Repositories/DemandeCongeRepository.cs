using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Output;
using Personnel.Infrastructure.Dtos;
using Personnel.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Personnel.Infrastructure.Repositories;

public sealed class DemandeCongeRepository : IDemandeCongeRepository
{
    private readonly AuditRepository<DemandeCongeDto> _repo;

    public DemandeCongeRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<DemandeCongeDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<DemandeCongeDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<DemandeConge?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : DemandeCongeMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<DemandeConge>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(DemandeCongeMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<DemandeConge>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(name ILIKE @Search OR motif ILIKE @Search OR statut_demande ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        if (typeId.HasValue)
        {
            conditions.Add("type_id = @TypeId");
            parameters["TypeId"] = typeId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<DemandeCongeDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(DemandeCongeMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<DemandeConge>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<DemandeConge> AddAsync(DemandeConge entity, CancellationToken ct = default)
    {
        var dto = DemandeCongeMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return DemandeCongeMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(DemandeConge entity, CancellationToken ct = default)
    {
        var dto = DemandeCongeMapper.ToDto(entity);
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
