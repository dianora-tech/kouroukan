using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Inscriptions.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Inscriptions.Domain.Services;

/// <summary>
/// Service metier pour les transferts d'eleves.
/// </summary>
public sealed class TransfertService : ITransfertService
{
    private const string Exchange = "kouroukan.events";
    private readonly ITransfertRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<TransfertService> _logger;

    public TransfertService(
        ITransfertRepository repository,
        IMessagePublisher publisher,
        ILogger<TransfertService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Transfert?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Transfert>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Transfert>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Transfert> CreateAsync(Transfert entity, CancellationToken ct = default)
    {
        if (entity.CompanyOrigineId == entity.CompanyCibleId)
            throw new InvalidOperationException("L'etablissement d'origine et l'etablissement cible doivent etre differents.");

        entity.Statut = "EnAttente";
        entity.DateDemande = DateTime.UtcNow;

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Transfert cree avec l'id {Id} pour l'eleve {EleveId}.", created.Id, created.EleveId);

        var evt = new EntityCreatedEvent<Transfert>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.transfert", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> AcceptAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (entity is null) return false;

        if (entity.Statut != "EnAttente")
            throw new InvalidOperationException("Seul un transfert en attente peut etre accepte.");

        entity.Statut = "Accepte";
        entity.DateTraitement = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Transfert {Id} accepte.", id);

            var evt = new EntityUpdatedEvent<Transfert>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.transfert.accepted", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> RejectAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (entity is null) return false;

        if (entity.Statut != "EnAttente")
            throw new InvalidOperationException("Seul un transfert en attente peut etre refuse.");

        entity.Statut = "Refuse";
        entity.DateTraitement = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Transfert {Id} refuse.", id);

            var evt = new EntityUpdatedEvent<Transfert>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.transfert.rejected", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> CompleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (entity is null) return false;

        if (entity.Statut != "Accepte")
            throw new InvalidOperationException("Seul un transfert accepte peut etre complete.");

        entity.Statut = "Complete";
        entity.DateTraitement = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Transfert {Id} complete.", id);

            var evt = new EntityUpdatedEvent<Transfert>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.transfert.completed", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
