using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Bde.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Bde.Domain.Services;

/// <summary>
/// Service metier pour les evenements.
/// </summary>
public sealed class EvenementService : IEvenementService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] StatutsValides = ["Planifie", "Valide", "EnCours", "Termine", "Annule"];

    private readonly IEvenementRepository _repository;
    private readonly IAssociationRepository _associationRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<EvenementService> _logger;

    public EvenementService(
        IEvenementRepository repository,
        IAssociationRepository associationRepository,
        IMessagePublisher publisher,
        ILogger<EvenementService> logger)
    {
        _repository = repository;
        _associationRepository = associationRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Evenement?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Evenement>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Evenement>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Evenement> CreateAsync(Evenement entity, CancellationToken ct = default)
    {
        if (!StatutsValides.Contains(entity.StatutEvenement))
            throw new InvalidOperationException(
                $"Statut d'evenement invalide : '{entity.StatutEvenement}'.");

        if (!await _associationRepository.ExistsAsync(entity.AssociationId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"L'association avec l'id {entity.AssociationId} n'existe pas.");

        if (entity.Capacite.HasValue && entity.NombreInscrits > entity.Capacite.Value)
            throw new InvalidOperationException("Le nombre d'inscrits depasse la capacite maximale.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Evenement {Id} cree pour l'association {AssociationId}.",
            created.Id, created.AssociationId);

        var evt = new EntityCreatedEvent<Evenement>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.evenement", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Evenement entity, CancellationToken ct = default)
    {
        if (!StatutsValides.Contains(entity.StatutEvenement))
            throw new InvalidOperationException(
                $"Statut d'evenement invalide : '{entity.StatutEvenement}'.");

        if (entity.Capacite.HasValue && entity.NombreInscrits > entity.Capacite.Value)
            throw new InvalidOperationException("Le nombre d'inscrits depasse la capacite maximale.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Evenement {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Evenement>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.evenement", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Evenement {Id} supprime.", id);

            var evt = new EntityDeletedEvent<Evenement>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.evenement", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
