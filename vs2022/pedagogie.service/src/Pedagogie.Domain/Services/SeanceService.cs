using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Pedagogie.Domain.Services;

/// <summary>
/// Service metier pour les seances.
/// </summary>
public sealed class SeanceService : ISeanceService
{
    private const string Exchange = "kouroukan.events";
    private readonly ISeanceRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<SeanceService> _logger;

    public SeanceService(
        ISeanceRepository repository,
        IMessagePublisher publisher,
        ILogger<SeanceService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Seance?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Seance>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Seance>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Seance> CreateAsync(Seance entity, CancellationToken ct = default)
    {
        if (entity.HeureFin <= entity.HeureDebut)
            throw new InvalidOperationException("L'heure de fin doit etre posterieure a l'heure de debut.");

        if (entity.JourSemaine < 1 || entity.JourSemaine > 6)
            throw new InvalidOperationException("Le jour de la semaine doit etre compris entre 1 (Lundi) et 6 (Samedi).");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Seance {Name} creee avec l'id {Id}.", created.Name, created.Id);

        var evt = new EntityCreatedEvent<Seance>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.seance", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Seance entity, CancellationToken ct = default)
    {
        if (entity.HeureFin <= entity.HeureDebut)
            throw new InvalidOperationException("L'heure de fin doit etre posterieure a l'heure de debut.");

        if (entity.JourSemaine < 1 || entity.JourSemaine > 6)
            throw new InvalidOperationException("Le jour de la semaine doit etre compris entre 1 (Lundi) et 6 (Samedi).");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Seance {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Seance>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.seance", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Seance {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Seance>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.seance", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
