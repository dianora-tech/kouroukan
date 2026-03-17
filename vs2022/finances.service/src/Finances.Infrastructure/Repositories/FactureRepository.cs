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
/// Implementation du repository pour les factures avec audit automatique.
/// </summary>
public sealed class FactureRepository : IFactureRepository
{
    private readonly AuditRepository<FactureDto> _repo;

    public FactureRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<FactureDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<FactureDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    /// <inheritdoc />
    public async Task<Facture?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : FactureMapper.ToEntity(dto);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Facture>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(FactureMapper.ToEntity).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<PagedResult<Facture>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(numero_facture ILIKE @Search OR statut_facture ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        if (typeId.HasValue)
        {
            conditions.Add("type_id = @TypeId");
            parameters["TypeId"] = typeId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<FactureDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(FactureMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Facture>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    /// <inheritdoc />
    public async Task<Facture> AddAsync(Facture entity, CancellationToken ct = default)
    {
        var dto = FactureMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return FactureMapper.ToEntity(created);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Facture entity, CancellationToken ct = default)
    {
        var dto = FactureMapper.ToDto(entity);
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
    public async Task<Facture?> GetByNumeroFactureAsync(string numeroFacture, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM finances.factures WHERE numero_facture = @NumeroFacture AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, new { NumeroFacture = numeroFacture }, ct).ConfigureAwait(false);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : FactureMapper.ToEntity(dto);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Facture>> GetByEleveIdAsync(int eleveId, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM finances.factures WHERE eleve_id = @EleveId AND is_deleted = FALSE ORDER BY created_at DESC";
        var dtos = await _repo.GetWithQueryAsync(sql, new { EleveId = eleveId }, ct).ConfigureAwait(false);
        return dtos.Select(FactureMapper.ToEntity).ToList().AsReadOnly();
    }
}
