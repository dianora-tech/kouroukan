using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Inscriptions.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Inscriptions.Domain.Services;

/// <summary>
/// Service metier pour les eleves.
/// </summary>
public sealed class EleveService : IEleveService
{
    private const string Exchange = "kouroukan.events";
    private readonly IEleveRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<EleveService> _logger;

    public EleveService(
        IEleveRepository repository,
        IMessagePublisher publisher,
        ILogger<EleveService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Eleve?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Eleve>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Eleve>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Eleve> CreateAsync(Eleve entity, CancellationToken ct = default)
    {
        var existing = await _repository.GetByMatriculeAsync(entity.NumeroMatricule, ct).ConfigureAwait(false);
        if (existing is not null)
            throw new InvalidOperationException($"Un eleve avec le matricule '{entity.NumeroMatricule}' existe deja.");

        if (entity.Genre is not ("M" or "F"))
            throw new InvalidOperationException("Le genre doit etre 'M' ou 'F'.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Eleve {FirstName} {LastName} cree avec l'id {Id}.",
            created.FirstName, created.LastName, created.Id);

        var evt = new EntityCreatedEvent<Eleve>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.eleve", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Eleve entity, CancellationToken ct = default)
    {
        if (entity.Genre is not ("M" or "F"))
            throw new InvalidOperationException("Le genre doit etre 'M' ou 'F'.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Eleve {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Eleve>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.eleve", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Eleve {Id} supprime.", id);

            var evt = new EntityDeletedEvent<Eleve>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.eleve", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
