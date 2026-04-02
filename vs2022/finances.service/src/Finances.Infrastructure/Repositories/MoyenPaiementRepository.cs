using Finances.Domain.Entities;
using Finances.Domain.Ports.Output;
using Finances.Infrastructure.Dtos;
using Finances.Infrastructure.Mappers;
using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Finances.Infrastructure.Repositories;

/// <summary>
/// Implementation du repository pour les moyens de paiement avec audit automatique.
/// </summary>
public sealed class MoyenPaiementRepository : IMoyenPaiementRepository
{
    private readonly AuditRepository<MoyenPaiementDto> _repo;

    public MoyenPaiementRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<MoyenPaiementDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<MoyenPaiementDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<MoyenPaiement?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : MoyenPaiementMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<MoyenPaiement>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(MoyenPaiementMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<MoyenPaiement>> GetPagedAsync(
        int page, int pageSize, int? companyId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (companyId.HasValue)
        {
            conditions.Add("company_id = @CompanyId");
            parameters["CompanyId"] = companyId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<MoyenPaiementDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(MoyenPaiementMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<MoyenPaiement>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<MoyenPaiement> AddAsync(MoyenPaiement entity, CancellationToken ct = default)
    {
        var dto = MoyenPaiementMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return MoyenPaiementMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(MoyenPaiement entity, CancellationToken ct = default)
    {
        var dto = MoyenPaiementMapper.ToDto(entity);
        return await _repo.UpdateAsync(dto, ct).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is not null;
    }
}
