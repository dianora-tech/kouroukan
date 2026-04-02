using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using Finances.Domain.Ports.Output;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace Finances.Domain.Services;

/// <summary>
/// Logique metier pour la gestion des moyens de paiement.
/// </summary>
public sealed class MoyenPaiementService : IMoyenPaiementService
{
    private const string Exchange = "kouroukan.events";
    private readonly IMoyenPaiementRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<MoyenPaiementService> _logger;

    public MoyenPaiementService(
        IMoyenPaiementRepository repository,
        IMessagePublisher publisher,
        ILogger<MoyenPaiementService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<MoyenPaiement?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MoyenPaiement>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<MoyenPaiement>> GetPagedAsync(
        int page, int pageSize, int? companyId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, companyId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<MoyenPaiement> CreateAsync(MoyenPaiement entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Operateur))
            throw new InvalidOperationException("L'operateur est obligatoire.");

        entity.EstActif = true;

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("MoyenPaiement {Libelle} cree avec l'id {Id} pour l'etablissement {CompanyId}.",
            created.Libelle, created.Id, created.CompanyId);

        var evt = new EntityCreatedEvent<MoyenPaiement>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.moyen-paiement", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(MoyenPaiement entity, CancellationToken ct = default)
    {
        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("MoyenPaiement {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<MoyenPaiement>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.moyen-paiement", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("MoyenPaiement {Id} supprime.", id);

            var evt = new EntityDeletedEvent<MoyenPaiement>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.moyen-paiement", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
