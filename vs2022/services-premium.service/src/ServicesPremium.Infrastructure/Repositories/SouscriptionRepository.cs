using Dapper;
using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Output;
using ServicesPremium.Infrastructure.Dtos;
using ServicesPremium.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ServicesPremium.Infrastructure.Repositories;

public sealed class SouscriptionRepository : ISouscriptionRepository
{
    private readonly AuditRepository<SouscriptionDto> _repo;

    public SouscriptionRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<SouscriptionDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<SouscriptionDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Souscription?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : SouscriptionMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Souscription>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(SouscriptionMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Souscription>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "(name ILIKE @Search OR statut_souscription ILIKE @Search)";
            dynamicParams.Add("Search", $"%{search}%");
        }

        var spec = new SimpleSpecification<SouscriptionDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(SouscriptionMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Souscription>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Souscription> AddAsync(Souscription entity, CancellationToken ct = default)
    {
        var dto = SouscriptionMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return SouscriptionMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Souscription entity, CancellationToken ct = default)
    {
        var dto = SouscriptionMapper.ToDto(entity);
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
