using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Output;
using Evaluations.Infrastructure.Dtos;
using Evaluations.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Evaluations.Infrastructure.Repositories;

public sealed class BulletinRepository : IBulletinRepository
{
    private readonly AuditRepository<BulletinDto> _repo;

    public BulletinRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<BulletinDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<BulletinDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Bulletin?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : BulletinMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Bulletin>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(BulletinMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Bulletin>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "appreciation ILIKE @Search OR name ILIKE @Search";
            parameters = new { Search = $"%{search}%" };
        }

        var spec = new SimpleSpecification<BulletinDto>(
            where, parameters,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(BulletinMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Bulletin>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Bulletin> AddAsync(Bulletin entity, CancellationToken ct = default)
    {
        var dto = BulletinMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return BulletinMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Bulletin entity, CancellationToken ct = default)
    {
        var dto = BulletinMapper.ToDto(entity);
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

    public async Task<Bulletin?> GetByEleveTrimestreAsync(int eleveId, int trimestre, int anneeScolaireId, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM evaluations.bulletins WHERE eleve_id = @EleveId AND trimestre = @Trimestre AND annee_scolaire_id = @AnneeScolaireId AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, new { EleveId = eleveId, Trimestre = trimestre, AnneeScolaireId = anneeScolaireId }, ct).ConfigureAwait(false);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : BulletinMapper.ToEntity(dto);
    }
}
