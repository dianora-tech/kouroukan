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

public sealed class ClasseRepository : IClasseRepository
{
    private readonly AuditRepository<ClasseDto> _repo;

    public ClasseRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<ClasseDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<ClasseDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<Classe?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : ClasseMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Classe>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(ClasseMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Classe>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        var where = string.Empty;
        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            where = "name ILIKE @Search";
            parameters = new { Search = $"%{search}%" };
        }

        var spec = new SimpleSpecification<ClasseDto>(
            where, parameters,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(ClasseMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Classe>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Classe> AddAsync(Classe entity, CancellationToken ct = default)
    {
        var dto = ClasseMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return ClasseMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Classe entity, CancellationToken ct = default)
    {
        var dto = ClasseMapper.ToDto(entity);
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
