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

public sealed class AnneeScolaireRepository : IAnneeScolaireRepository
{
    private readonly AuditRepository<AnneeScolaireDto> _repo;

    public AnneeScolaireRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<AnneeScolaireDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<AnneeScolaireDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<AnneeScolaire?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : AnneeScolaireMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<AnneeScolaire>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(AnneeScolaireMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<AnneeScolaire>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "libelle ILIKE @Search";
            parameters = new { Search = $"%{search}%" };
        }

        var spec = new SimpleSpecification<AnneeScolaireDto>(
            where, parameters,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(AnneeScolaireMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<AnneeScolaire>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<AnneeScolaire> AddAsync(AnneeScolaire entity, CancellationToken ct = default)
    {
        var dto = AnneeScolaireMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return AnneeScolaireMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(AnneeScolaire entity, CancellationToken ct = default)
    {
        var dto = AnneeScolaireMapper.ToDto(entity);
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
