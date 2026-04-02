using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Output;
using Inscriptions.Infrastructure.Dtos;
using Inscriptions.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Inscriptions.Infrastructure.Repositories;

public sealed class RadiationRepository : IRadiationRepository
{
    private readonly AuditRepository<RadiationDto> _repo;

    public RadiationRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<RadiationDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<RadiationDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Radiation?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : RadiationMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Radiation>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(RadiationMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Radiation>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("motif ILIKE @Search");
            parameters["Search"] = $"%{search}%";
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<RadiationDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(RadiationMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Radiation>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Radiation> AddAsync(Radiation entity, CancellationToken ct = default)
    {
        var dto = RadiationMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return RadiationMapper.ToEntity(created);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is not null;
    }
}
