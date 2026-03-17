using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Support.Domain.Ports.Output;

namespace Support.Domain.Services;

/// <summary>
/// Service metier pour la gestion des tickets de support.
/// </summary>
public sealed class TicketService : ITicketService
{
    private const string Exchange = "kouroukan.events";
    private readonly ITicketRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<TicketService> _logger;

    public TicketService(
        ITicketRepository repository,
        IMessagePublisher publisher,
        ILogger<TicketService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Ticket>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct);
    }

    /// <inheritdoc />
    public async Task<Ticket> CreateAsync(Ticket entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Sujet))
            throw new InvalidOperationException("Le sujet du ticket est obligatoire.");

        if (string.IsNullOrWhiteSpace(entity.Contenu))
            throw new InvalidOperationException("Le contenu du ticket est obligatoire.");

        if (entity.Priorite is not ("Basse" or "Moyenne" or "Haute" or "Critique"))
            throw new InvalidOperationException("La priorite doit etre 'Basse', 'Moyenne', 'Haute' ou 'Critique'.");

        entity.StatutTicket = "Ouvert";

        var created = await _repository.AddAsync(entity, ct);
        _logger.LogInformation("Ticket '{Sujet}' cree avec l'id {Id}.", created.Sujet, created.Id);

        var evt = new EntityCreatedEvent<Ticket>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.ticket", cancellationToken: ct);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Ticket entity, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(entity.Id, ct);
        if (existing is null)
            return false;

        if (entity.StatutTicket is not ("Ouvert" or "EnCours" or "EnAttente" or "Resolu" or "Ferme"))
            throw new InvalidOperationException("Le statut du ticket est invalide.");

        if (entity.NoteSatisfaction is not null and (< 1 or > 5))
            throw new InvalidOperationException("La note de satisfaction doit etre entre 1 et 5.");

        if (entity.StatutTicket is "Resolu" or "Ferme" && existing.StatutTicket is not ("Resolu" or "Ferme"))
            entity.DateResolution = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity, ct);
        _logger.LogInformation("Ticket {Id} mis a jour.", entity.Id);

        var evt = new EntityUpdatedEvent<Ticket>(entity) { EntityId = entity.Id, UserId = entity.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.updated.ticket", cancellationToken: ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct);
        if (result)
        {
            _logger.LogInformation("Ticket {Id} supprime.", id);
            var evt = new EntityDeletedEvent<Ticket> { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.ticket", cancellationToken: ct);
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ReponseTicket>> GetReponsesAsync(int ticketId, CancellationToken ct = default)
    {
        return await _repository.GetReponsesAsync(ticketId, ct);
    }

    /// <inheritdoc />
    public async Task<ReponseTicket> AddReponseAsync(ReponseTicket reponse, CancellationToken ct = default)
    {
        var ticket = await _repository.GetByIdAsync(reponse.TicketId, ct)
            ?? throw new KeyNotFoundException($"Ticket {reponse.TicketId} introuvable.");

        if (string.IsNullOrWhiteSpace(reponse.Contenu))
            throw new InvalidOperationException("Le contenu de la reponse est obligatoire.");

        var created = await _repository.AddReponseAsync(reponse, ct);
        _logger.LogInformation("Reponse ajoutee au ticket {TicketId} (IA: {EstIA}).", reponse.TicketId, reponse.EstReponseIA);

        var evt = new EntityCreatedEvent<ReponseTicket>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.reponseticket", cancellationToken: ct);

        return created;
    }
}
