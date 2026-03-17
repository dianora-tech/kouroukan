using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using Personnel.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Personnel.Domain.Services;

/// <summary>
/// Service metier pour les demandes de conge.
/// </summary>
public sealed class DemandeCongeService : IDemandeCongeService
{
    private const string Exchange = "kouroukan.events";
    private readonly IDemandeCongeRepository _repository;
    private readonly IEnseignantRepository _enseignantRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<DemandeCongeService> _logger;

    public DemandeCongeService(
        IDemandeCongeRepository repository,
        IEnseignantRepository enseignantRepository,
        IMessagePublisher publisher,
        ILogger<DemandeCongeService> logger)
    {
        _repository = repository;
        _enseignantRepository = enseignantRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<DemandeConge?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DemandeConge>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<DemandeConge>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<DemandeConge> CreateAsync(DemandeConge entity, CancellationToken ct = default)
    {
        var enseignant = await _enseignantRepository.GetByIdAsync(entity.EnseignantId, ct).ConfigureAwait(false);
        if (enseignant is null)
            throw new InvalidOperationException($"L'enseignant {entity.EnseignantId} n'existe pas.");

        if (entity.DateFin <= entity.DateDebut)
            throw new InvalidOperationException("La date de fin doit etre posterieure a la date de debut.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Demande de conge creee avec l'id {Id} pour l'enseignant {EnseignantId}.",
            created.Id, created.EnseignantId);

        var evt = new EntityCreatedEvent<DemandeConge>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.demandeconge", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(DemandeConge entity, CancellationToken ct = default)
    {
        if (entity.DateFin <= entity.DateDebut)
            throw new InvalidOperationException("La date de fin doit etre posterieure a la date de debut.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Demande de conge {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<DemandeConge>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.demandeconge", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Demande de conge {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<DemandeConge>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.demandeconge", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
