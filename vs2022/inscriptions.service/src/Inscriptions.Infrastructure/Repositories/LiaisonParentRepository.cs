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

public sealed class LiaisonParentRepository : ILiaisonParentRepository
{
    private readonly AuditRepository<LiaisonParentDto> _repo;

    public LiaisonParentRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<LiaisonParentDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<LiaisonParentDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<LiaisonParent?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : LiaisonParentMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<LiaisonParent>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(LiaisonParentMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<LiaisonParent>> GetPagedAsync(
        int page, int pageSize, int? parentUserId, int? companyId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (parentUserId.HasValue)
        {
            conditions.Add("parent_user_id = @ParentUserId");
            parameters["ParentUserId"] = parentUserId.Value;
        }

        if (companyId.HasValue)
        {
            conditions.Add("company_id = @CompanyId");
            parameters["CompanyId"] = companyId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<LiaisonParentDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(LiaisonParentMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<LiaisonParent>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<LiaisonParent> AddAsync(LiaisonParent entity, CancellationToken ct = default)
    {
        var dto = LiaisonParentMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return LiaisonParentMapper.ToEntity(created);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is not null;
    }
}
