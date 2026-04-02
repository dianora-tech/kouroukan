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

public sealed class TransfertRepository : ITransfertRepository
{
    private readonly AuditRepository<TransfertDto> _repo;

    public TransfertRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<TransfertDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<TransfertDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Transfert?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : TransfertMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Transfert>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(TransfertMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Transfert>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(statut ILIKE @Search OR motif ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<TransfertDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(TransfertMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Transfert>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Transfert> AddAsync(Transfert entity, CancellationToken ct = default)
    {
        var dto = TransfertMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return TransfertMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Transfert entity, CancellationToken ct = default)
    {
        var dto = TransfertMapper.ToDto(entity);
        return await _repo.UpdateAsync(dto, ct).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is not null;
    }
}
