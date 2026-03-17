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

public sealed class EnseignantRepository : IEnseignantRepository
{
    private readonly AuditRepository<EnseignantDto> _repo;

    public EnseignantRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<EnseignantDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<EnseignantDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Enseignant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : EnseignantMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Enseignant>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(EnseignantMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Enseignant>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(name ILIKE @Search OR matricule ILIKE @Search OR specialite ILIKE @Search OR telephone ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        if (typeId.HasValue)
        {
            conditions.Add("type_id = @TypeId");
            parameters["TypeId"] = typeId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<EnseignantDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(EnseignantMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Enseignant>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Enseignant> AddAsync(Enseignant entity, CancellationToken ct = default)
    {
        var dto = EnseignantMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return EnseignantMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Enseignant entity, CancellationToken ct = default)
    {
        var dto = EnseignantMapper.ToDto(entity);
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

    public async Task<Enseignant?> GetByMatriculeAsync(string matricule, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM personnel.enseignants WHERE matricule = @Matricule AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, new { Matricule = matricule }, ct).ConfigureAwait(false);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : EnseignantMapper.ToEntity(dto);
    }
}
