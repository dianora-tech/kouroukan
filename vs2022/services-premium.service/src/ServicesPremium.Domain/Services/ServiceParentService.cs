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
/// Logique metier pour les services parents (premium).
/// </summary>
public sealed class ServiceParentService : IServiceParentService
{
    private const string Exchange = "kouroukan.events";

    private static readonly HashSet<string> PeriodicitesValides = ["Mensuel", "Annuel", "Unite"];

    private readonly IServiceParentRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<ServiceParentService> _logger;

    public ServiceParentService(
        IServiceParentRepository repository,
        IMessagePublisher publisher,
        ILogger<ServiceParentService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ServiceParent?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceParent>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct);
    }

    /// <inheritdoc />
    public async Task<PagedResult<ServiceParent>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct);
    }

    /// <inheritdoc />
    public async Task<ServiceParent> CreateAsync(ServiceParent entity, CancellationToken ct = default)
    {
        // Validation de la periodicite
        if (!PeriodicitesValides.Contains(entity.Periodicite))
            throw new InvalidOperationException($"Periodicite invalide : {entity.Periodicite}. Valeurs acceptees : {string.Join(", ", PeriodicitesValides)}");

        // Validation du tarif
        if (entity.Tarif < 0)
            throw new InvalidOperationException("Le tarif ne peut pas etre negatif.");

        // Validation de la periode d'essai
        if (entity.PeriodeEssaiJours.HasValue && entity.PeriodeEssaiJours.Value < 0)
            throw new InvalidOperationException("La periode d'essai ne peut pas etre negative.");

        var created = await _repository.AddAsync(entity, ct);

        _logger.LogInformation("ServiceParent {Id} cree — Code: {Code}, Tarif: {Tarif} GNF",
            created.Id, created.Code, created.Tarif);

        var evt = new EntityCreatedEvent<ServiceParent>(created)
        {
            EntityId = created.Id,
            UserId = created.UserId
        };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.serviceparent", cancellationToken: ct);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(ServiceParent entity, CancellationToken ct = default)
    {
        // Validation de la periodicite
        if (!PeriodicitesValides.Contains(entity.Periodicite))
            throw new InvalidOperationException($"Periodicite invalide : {entity.Periodicite}. Valeurs acceptees : {string.Join(", ", PeriodicitesValides)}");

        // Validation du tarif
        if (entity.Tarif < 0)
            throw new InvalidOperationException("Le tarif ne peut pas etre negatif.");

        if (!await _repository.ExistsAsync(entity.Id, ct))
            throw new KeyNotFoundException($"ServiceParent {entity.Id} introuvable.");

        var result = await _repository.UpdateAsync(entity, ct);

        if (result)
        {
            _logger.LogInformation("ServiceParent {Id} mis a jour", entity.Id);

            var evt = new EntityUpdatedEvent<ServiceParent>(entity)
            {
                EntityId = entity.Id,
                UserId = entity.UserId
            };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.serviceparent", cancellationToken: ct);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            throw new KeyNotFoundException($"ServiceParent {id} introuvable.");

        var result = await _repository.DeleteAsync(id, ct);

        if (result)
        {
            _logger.LogInformation("ServiceParent {Id} supprime (soft delete)", id);

            var evt = new EntityDeletedEvent<ServiceParent>
            {
                EntityId = id
            };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.serviceparent", cancellationToken: ct);
        }

        return result;
    }
}
