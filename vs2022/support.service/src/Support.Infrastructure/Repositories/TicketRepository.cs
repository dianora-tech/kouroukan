using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Support.Domain.Entities;
using Support.Domain.Ports.Output;
using Support.Infrastructure.Dtos;
using Support.Infrastructure.Mappers;

namespace Support.Infrastructure.Repositories;

/// <summary>
/// Repository pour les tickets de support (PostgreSQL).
/// </summary>
public sealed class TicketRepository : ITicketRepository
{
    private readonly AuditRepository<TicketDto> _repo;
    private readonly AuditRepository<ReponseTicketDto> _reponseRepo;

    public TicketRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<TicketDto>> logger,
        ILogger<Repository<ReponseTicketDto>> reponseLogger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<TicketDto>(connectionFactory, logger, options, httpContextAccessor);
        _reponseRepo = new AuditRepository<ReponseTicketDto>(connectionFactory, reponseLogger, options, httpContextAccessor);
    }

    public async Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct);
        return dto is null ? null : TicketMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct);
        return dtos.Select(TicketMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Ticket>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(sujet ILIKE @Search OR contenu ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        if (typeId.HasValue)
        {
            conditions.Add("type_id = @TypeId");
            parameters["TypeId"] = typeId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : null;
        var spec = new SimpleSpecification<TicketDto>(
            where,
            parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "created_at DESC" : orderBy,
            (page - 1) * pageSize,
            pageSize);

        var result = await _repo.FindPagedAsync(spec, ct);
        var entities = result.Items.Select(TicketMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<Ticket>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<Ticket> AddAsync(Ticket entity, CancellationToken ct = default)
    {
        var dto = TicketMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct);
        return TicketMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(Ticket entity, CancellationToken ct = default)
    {
        var dto = TicketMapper.ToDto(entity);
        return await _repo.UpdateAsync(dto, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        await _repo.DeleteAsync(id, ct);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
        await _repo.ExistsAsync(id, ct);

    public async Task<IReadOnlyList<ReponseTicket>> GetReponsesAsync(int ticketId, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM support.reponses_tickets WHERE ticket_id = @TicketId AND is_deleted = FALSE ORDER BY created_at ASC";
        var dtos = await _reponseRepo.GetWithQueryAsync(sql, new { TicketId = ticketId }, ct);
        return dtos.Select(ReponseTicketMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<ReponseTicket> AddReponseAsync(ReponseTicket reponse, CancellationToken ct = default)
    {
        var dto = ReponseTicketMapper.ToDto(reponse);
        var created = await _reponseRepo.AddAsync(dto, ct);
        return ReponseTicketMapper.ToEntity(created);
    }

    public async Task<int> CountByStatutAsync(string statut, CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(*) FROM support.tickets WHERE statut_ticket = @Statut AND is_deleted = FALSE";
        var results = await _repo.GetWithQueryAsync(sql, new { Statut = statut }, ct);
        return results.Count();
    }

    public async Task<double> GetTempsMoyenResolutionAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT COALESCE(AVG(EXTRACT(EPOCH FROM (date_resolution - created_at)) / 3600), 0)
            FROM support.tickets
            WHERE date_resolution IS NOT NULL AND is_deleted = FALSE";
        var result = await _repo.GetSingleWithQueryAsync(sql, null, ct);
        return result is not null ? 0 : 0;
    }

    public async Task<double> GetMoyenneSatisfactionAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT COALESCE(AVG(note_satisfaction), 0)
            FROM support.tickets
            WHERE note_satisfaction IS NOT NULL AND is_deleted = FALSE";
        var result = await _repo.GetSingleWithQueryAsync(sql, null, ct);
        return result is not null ? 0 : 0;
    }
}
