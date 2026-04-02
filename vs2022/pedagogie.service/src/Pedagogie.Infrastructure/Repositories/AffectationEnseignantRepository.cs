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

public sealed class AffectationEnseignantRepository : IAffectationEnseignantRepository
{
    private readonly AuditRepository<AffectationEnseignantDto> _repo;

    public AffectationEnseignantRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<AffectationEnseignantDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<AffectationEnseignantDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<AffectationEnseignant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : AffectationEnseignantMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<AffectationEnseignant>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(AffectationEnseignantMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<AffectationEnseignant>> GetPagedAsync(
        int page, int pageSize, int? liaisonId, int? classeId, int? matiereId, int? anneeScolaireId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (liaisonId.HasValue)
        {
            conditions.Add("liaison_id = @LiaisonId");
            parameters["LiaisonId"] = liaisonId.Value;
        }

        if (classeId.HasValue)
        {
            conditions.Add("classe_id = @ClasseId");
            parameters["ClasseId"] = classeId.Value;
        }

        if (matiereId.HasValue)
        {
            conditions.Add("matiere_id = @MatiereId");
            parameters["MatiereId"] = matiereId.Value;
        }

        if (anneeScolaireId.HasValue)
        {
            conditions.Add("annee_scolaire_id = @AnneeScolaireId");
            parameters["AnneeScolaireId"] = anneeScolaireId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<AffectationEnseignantDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(AffectationEnseignantMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<AffectationEnseignant>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<AffectationEnseignant> AddAsync(AffectationEnseignant entity, CancellationToken ct = default)
    {
        var dto = AffectationEnseignantMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return AffectationEnseignantMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(AffectationEnseignant entity, CancellationToken ct = default)
    {
        var dto = AffectationEnseignantMapper.ToDto(entity);
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
