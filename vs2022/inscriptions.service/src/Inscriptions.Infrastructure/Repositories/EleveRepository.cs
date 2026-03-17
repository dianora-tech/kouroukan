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

public sealed class EleveRepository : IEleveRepository
{
    private readonly AuditRepository<EleveDto> _repo;

    public EleveRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<EleveDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<EleveDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Eleve?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : EleveMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Eleve>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(EleveMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Eleve>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "first_name ILIKE @Search OR last_name ILIKE @Search OR numero_matricule ILIKE @Search";
            parameters = new { Search = $"%{search}%" };
        }

        var spec = new SimpleSpecification<EleveDto>(
            where, parameters,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(EleveMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Eleve>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Eleve> AddAsync(Eleve entity, CancellationToken ct = default)
    {
        var dto = EleveMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return EleveMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Eleve entity, CancellationToken ct = default)
    {
        var dto = EleveMapper.ToDto(entity);
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

    public async Task<Eleve?> GetByMatriculeAsync(string matricule, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM inscriptions.eleves WHERE numero_matricule = @Matricule AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, new { Matricule = matricule }, ct).ConfigureAwait(false);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : EleveMapper.ToEntity(dto);
    }
}
