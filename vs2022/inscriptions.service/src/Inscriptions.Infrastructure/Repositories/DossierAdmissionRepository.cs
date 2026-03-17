using Dapper;
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

public sealed class DossierAdmissionRepository : IDossierAdmissionRepository
{
    private readonly AuditRepository<DossierAdmissionDto> _repo;
    private readonly IDbConnectionFactory _connectionFactory;

    public DossierAdmissionRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<DossierAdmissionDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _repo = new AuditRepository<DossierAdmissionDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<DossierAdmission?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : DossierAdmissionMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<DossierAdmission>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(DossierAdmissionMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<DossierAdmission>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        var dynamicParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "statut_dossier ILIKE @Search OR etape_actuelle ILIKE @Search";
            dynamicParams.Add("Search", $"%{search}%");
        }

        if (typeId.HasValue)
        {
            where += string.IsNullOrEmpty(where) ? "type_id = @TypeId" : " AND type_id = @TypeId";
            dynamicParams.Add("TypeId", typeId.Value);
        }

        var spec = new SimpleSpecification<DossierAdmissionDto>(
            where, dynamicParams,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(DossierAdmissionMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<DossierAdmission>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<DossierAdmission> AddAsync(DossierAdmission entity, CancellationToken ct = default)
    {
        var dto = DossierAdmissionMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return DossierAdmissionMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(DossierAdmission entity, CancellationToken ct = default)
    {
        var dto = DossierAdmissionMapper.ToDto(entity);
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

    public async Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT id, name, description FROM inscriptions.type_dossiers_admission WHERE is_deleted = FALSE ORDER BY name";
        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: ct);
        var result = await connection.QueryAsync<TypeDto>(command).ConfigureAwait(false);
        return result.AsList().AsReadOnly();
    }
}
