using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using Finances.Domain.Ports.Output;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace Finances.Domain.Services;

/// <summary>
/// Logique metier pour la gestion des factures.
/// </summary>
public sealed class FactureService : IFactureService
{
    private const string Exchange = "kouroukan.events";
    private readonly IFactureRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<FactureService> _logger;

    public FactureService(
        IFactureRepository repository,
        IMessagePublisher publisher,
        ILogger<FactureService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Facture?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Facture>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Facture>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Facture> CreateAsync(Facture entity, CancellationToken ct = default)
    {
        var existing = await _repository.GetByNumeroFactureAsync(entity.NumeroFacture, ct).ConfigureAwait(false);
        if (existing is not null)
            throw new InvalidOperationException($"Une facture avec le numero '{entity.NumeroFacture}' existe deja.");

        if (entity.MontantTotal < 0)
            throw new InvalidOperationException("Le montant total ne peut pas etre negatif.");

        entity.Solde = entity.MontantTotal - entity.MontantPaye;

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Facture {NumeroFacture} creee avec l'id {Id} pour l'eleve {EleveId}.",
            created.NumeroFacture, created.Id, created.EleveId);

        var evt = new EntityCreatedEvent<Facture>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.facture", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Facture entity, CancellationToken ct = default)
    {
        if (entity.MontantTotal < 0)
            throw new InvalidOperationException("Le montant total ne peut pas etre negatif.");

        entity.Solde = entity.MontantTotal - entity.MontantPaye;

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Facture {Id} ({NumeroFacture}) mise a jour.", entity.Id, entity.NumeroFacture);

            var evt = new EntityUpdatedEvent<Facture>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.facture", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Facture {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Facture>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.facture", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
