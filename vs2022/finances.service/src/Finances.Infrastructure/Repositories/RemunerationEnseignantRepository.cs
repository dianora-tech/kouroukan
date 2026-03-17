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
/// Implementation du repository pour les remunerations enseignants avec audit automatique.
/// </summary>
public sealed class RemunerationEnseignantRepository : IRemunerationEnseignantRepository
{
    private readonly AuditRepository<RemunerationEnseignantDto> _repo;

    public RemunerationEnseignantRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<RemunerationEnseignantDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<RemunerationEnseignantDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    /// <inheritdoc />
    public async Task<RemunerationEnseignant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : RemunerationEnseignantMapper.ToEntity(dto);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RemunerationEnseignant>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(RemunerationEnseignantMapper.ToEntity).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<PagedResult<RemunerationEnseignant>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "mode_remuneration ILIKE @Search OR statut_paiement ILIKE @Search OR CAST(enseignant_id AS TEXT) ILIKE @Search";
            parameters = new { Search = $"%{search}%" };
        }

        var spec = new SimpleSpecification<RemunerationEnseignantDto>(
            where, parameters,
            string.IsNullOrWhiteSpace(orderBy) ? "annee DESC, mois DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(RemunerationEnseignantMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<RemunerationEnseignant>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    /// <inheritdoc />
    public async Task<RemunerationEnseignant> AddAsync(RemunerationEnseignant entity, CancellationToken ct = default)
    {
        var dto = RemunerationEnseignantMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return RemunerationEnseignantMapper.ToEntity(created);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(RemunerationEnseignant entity, CancellationToken ct = default)
    {
        var dto = RemunerationEnseignantMapper.ToDto(entity);
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
    public async Task<RemunerationEnseignant?> GetByEnseignantMoisAnneeAsync(
        int enseignantId, int mois, int annee, CancellationToken ct = default)
    {
        const string sql = """
            SELECT * FROM finances.remunerations_enseignants
            WHERE enseignant_id = @EnseignantId AND mois = @Mois AND annee = @Annee AND is_deleted = FALSE
            """;
        var dtos = await _repo.GetWithQueryAsync(sql,
            new { EnseignantId = enseignantId, Mois = mois, Annee = annee }, ct).ConfigureAwait(false);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : RemunerationEnseignantMapper.ToEntity(dto);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RemunerationEnseignant>> GetByEnseignantIdAsync(
        int enseignantId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT * FROM finances.remunerations_enseignants
            WHERE enseignant_id = @EnseignantId AND is_deleted = FALSE
            ORDER BY annee DESC, mois DESC
            """;
        var dtos = await _repo.GetWithQueryAsync(sql, new { EnseignantId = enseignantId }, ct).ConfigureAwait(false);
        return dtos.Select(RemunerationEnseignantMapper.ToEntity).ToList().AsReadOnly();
    }
}
