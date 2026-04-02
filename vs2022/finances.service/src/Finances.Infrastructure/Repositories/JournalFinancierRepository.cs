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
/// Implementation du repository pour le journal financier avec audit automatique.
/// </summary>
public sealed class JournalFinancierRepository : IJournalFinancierRepository
{
    private readonly AuditRepository<JournalFinancierDto> _repo;

    public JournalFinancierRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<JournalFinancierDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<JournalFinancierDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<JournalFinancier?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is null ? null : JournalFinancierMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<JournalFinancier>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct).ConfigureAwait(false);
        return dtos.Select(JournalFinancierMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<JournalFinancier>> GetPagedAsync(
        int page, int pageSize, int? companyId, string? type, string? categorie,
        DateTime? dateDebut, DateTime? dateFin, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (companyId.HasValue)
        {
            conditions.Add("company_id = @CompanyId");
            parameters["CompanyId"] = companyId.Value;
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            conditions.Add("type = @Type");
            parameters["Type"] = type;
        }

        if (!string.IsNullOrWhiteSpace(categorie))
        {
            conditions.Add("categorie = @Categorie");
            parameters["Categorie"] = categorie;
        }

        if (dateDebut.HasValue)
        {
            conditions.Add("date_operation >= @DateDebut");
            parameters["DateDebut"] = dateDebut.Value;
        }

        if (dateFin.HasValue)
        {
            conditions.Add("date_operation <= @DateFin");
            parameters["DateFin"] = dateFin.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : string.Empty;

        var spec = new SimpleSpecification<JournalFinancierDto>(
            where, parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "date_operation DESC" : orderBy,
            (page - 1) * pageSize, pageSize);

        var result = await _repo.FindPagedAsync(spec, ct).ConfigureAwait(false);
        var entities = result.Items.Select(JournalFinancierMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<JournalFinancier>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<JournalFinancier> AddAsync(JournalFinancier entity, CancellationToken ct = default)
    {
        var dto = JournalFinancierMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct).ConfigureAwait(false);
        return JournalFinancierMapper.ToEntity(created);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct).ConfigureAwait(false);
        return dto is not null;
    }
}
