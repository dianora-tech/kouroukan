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
/// Implementation du repository pour les paiements avec audit automatique.
/// </summary>
public sealed class PaiementRepository : IPaiementRepository
{
    private readonly AuditRepository<PaiementDto> _repo;

    public PaiementRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<PaiementDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<PaiementDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    /// <inheritdoc />
    public async Task<Paiement?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : PaiementMapper.ToEntity(dto);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Paiement>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(PaiementMapper.ToEntity).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<PagedResult<Paiement>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(numero_recu ILIKE @Search OR moyen_paiement ILIKE @Search OR reference_mobile_money ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        if (typeId.HasValue)
        {
            conditions.Add("type_id = @TypeId");
            parameters["TypeId"] = typeId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<PaiementDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(PaiementMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Paiement>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    /// <inheritdoc />
    public async Task<Paiement> AddAsync(Paiement entity, CancellationToken ct = default)
    {
        var dto = PaiementMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return PaiementMapper.ToEntity(created);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Paiement entity, CancellationToken ct = default)
    {
        var dto = PaiementMapper.ToDto(entity);
        return await _repo.UpdateAsync(dto, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is not null;
    }

    /// <inheritdoc />
    public async Task<Paiement?> GetByNumeroRecuAsync(string numeroRecu, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM finances.paiements WHERE numero_recu = @NumeroRecu AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, new { NumeroRecu = numeroRecu }, ct).ConfigureAwait(false);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : PaiementMapper.ToEntity(dto);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Paiement>> GetByFactureIdAsync(int factureId, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM finances.paiements WHERE facture_id = @FactureId AND is_deleted = FALSE ORDER BY created_at DESC";
        var dtos = await _repo.GetWithQueryAsync(sql, new { FactureId = factureId }, ct).ConfigureAwait(false);
        return dtos.Select(PaiementMapper.ToEntity).ToList().AsReadOnly();
    }
}
