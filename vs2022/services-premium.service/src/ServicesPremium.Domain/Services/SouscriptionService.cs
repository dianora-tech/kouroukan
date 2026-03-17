using GnDapper.Entities;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;
using ServicesPremium.Domain.Ports.Output;

namespace ServicesPremium.Domain.Services;

/// <summary>
/// Logique metier pour les souscriptions aux services premium.
/// </summary>
public sealed class SouscriptionService : ISouscriptionService
{
    private const string Exchange = "kouroukan.events";

    private static readonly HashSet<string> StatutsValides = ["Active", "Expiree", "Resiliee", "Essai"];

    private readonly ISouscriptionRepository _repository;
    private readonly IServiceParentRepository _serviceParentRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<SouscriptionService> _logger;

    public SouscriptionService(
        ISouscriptionRepository repository,
        IServiceParentRepository serviceParentRepository,
        IMessagePublisher publisher,
        ILogger<SouscriptionService> logger)
    {
        _repository = repository;
        _serviceParentRepository = serviceParentRepository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Souscription?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Souscription>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Souscription>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct);
    }

    /// <inheritdoc />
    public async Task<Souscription> CreateAsync(Souscription entity, CancellationToken ct = default)
    {
        // Validation du statut
        if (!StatutsValides.Contains(entity.StatutSouscription))
            throw new InvalidOperationException($"Statut invalide : {entity.StatutSouscription}. Valeurs acceptees : {string.Join(", ", StatutsValides)}");

        // Verification de l'existence du service parent
        if (!await _serviceParentRepository.ExistsAsync(entity.ServiceParentId, ct))
            throw new KeyNotFoundException($"ServiceParent {entity.ServiceParentId} introuvable.");

        // Validation du montant
        if (entity.MontantPaye < 0)
            throw new InvalidOperationException("Le montant paye ne peut pas etre negatif.");

        // Validation des dates
        if (entity.DateFin.HasValue && entity.DateFin.Value < entity.DateDebut)
            throw new InvalidOperationException("La date de fin ne peut pas etre anterieure a la date de debut.");

        if (entity.DateProchainRenouvellement.HasValue && entity.DateProchainRenouvellement.Value < entity.DateDebut)
            throw new InvalidOperationException("La date de prochain renouvellement ne peut pas etre anterieure a la date de debut.");

        var created = await _repository.AddAsync(entity, ct);

        _logger.LogInformation("Souscription {Id} creee — ServiceParent: {ServiceParentId}, Parent: {ParentId}, Statut: {Statut}",
            created.Id, created.ServiceParentId, created.ParentId, created.StatutSouscription);

        var evt = new EntityCreatedEvent<Souscription>(created)
        {
            EntityId = created.Id,
            UserId = created.UserId
        };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.souscription", cancellationToken: ct);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Souscription entity, CancellationToken ct = default)
    {
        // Validation du statut
        if (!StatutsValides.Contains(entity.StatutSouscription))
            throw new InvalidOperationException($"Statut invalide : {entity.StatutSouscription}. Valeurs acceptees : {string.Join(", ", StatutsValides)}");

        if (!await _repository.ExistsAsync(entity.Id, ct))
            throw new KeyNotFoundException($"Souscription {entity.Id} introuvable.");

        // Verification de l'existence du service parent
        if (!await _serviceParentRepository.ExistsAsync(entity.ServiceParentId, ct))
            throw new KeyNotFoundException($"ServiceParent {entity.ServiceParentId} introuvable.");

        // Validation des dates
        if (entity.DateFin.HasValue && entity.DateFin.Value < entity.DateDebut)
            throw new InvalidOperationException("La date de fin ne peut pas etre anterieure a la date de debut.");

        var result = await _repository.UpdateAsync(entity, ct);

        if (result)
        {
            _logger.LogInformation("Souscription {Id} mise a jour — Statut: {Statut}", entity.Id, entity.StatutSouscription);

            var evt = new EntityUpdatedEvent<Souscription>(entity)
            {
                EntityId = entity.Id,
                UserId = entity.UserId
            };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.souscription", cancellationToken: ct);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            throw new KeyNotFoundException($"Souscription {id} introuvable.");

        var result = await _repository.DeleteAsync(id, ct);

        if (result)
        {
            _logger.LogInformation("Souscription {Id} supprimee (soft delete)", id);

            var evt = new EntityDeletedEvent<Souscription>
            {
                EntityId = id
            };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.souscription", cancellationToken: ct);
        }

        return result;
    }
}
