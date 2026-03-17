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
/// Implementation du repository pour les depenses avec audit automatique.
/// </summary>
public sealed class DepenseRepository : IDepenseRepository
{
    private readonly AuditRepository<DepenseDto> _repo;

    public DepenseRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<DepenseDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<DepenseDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    /// <inheritdoc />
    public async Task<Depense?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : DepenseMapper.ToEntity(dto);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Depense>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(DepenseMapper.ToEntity).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<PagedResult<Depense>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(motif_depense ILIKE @Search OR beneficiaire_nom ILIKE @Search OR numero_justificatif ILIKE @Search OR categorie ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        if (typeId.HasValue)
        {
            conditions.Add("type_id = @TypeId");
            parameters["TypeId"] = typeId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<DepenseDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(DepenseMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Depense>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    /// <inheritdoc />
    public async Task<Depense> AddAsync(Depense entity, CancellationToken ct = default)
    {
        var dto = DepenseMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return DepenseMapper.ToEntity(created);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Depense entity, CancellationToken ct = default)
    {
        var dto = DepenseMapper.ToDto(entity);
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
    public async Task<Depense?> GetByNumeroJustificatifAsync(string numeroJustificatif, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM finances.depenses WHERE numero_justificatif = @NumeroJustificatif AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, new { NumeroJustificatif = numeroJustificatif }, ct).ConfigureAwait(false);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : DepenseMapper.ToEntity(dto);
    }
}
