using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using Personnel.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Personnel.Domain.Services;

/// <summary>
/// Service metier pour les enseignants.
/// </summary>
public sealed class EnseignantService : IEnseignantService
{
    private const string Exchange = "kouroukan.events";
    private readonly IEnseignantRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<EnseignantService> _logger;

    public EnseignantService(
        IEnseignantRepository repository,
        IMessagePublisher publisher,
        ILogger<EnseignantService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Enseignant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Enseignant>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Enseignant>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Enseignant> CreateAsync(Enseignant entity, CancellationToken ct = default)
    {
        var existing = await _repository.GetByMatriculeAsync(entity.Matricule, ct).ConfigureAwait(false);
        if (existing is not null)
            throw new InvalidOperationException($"Un enseignant avec le matricule '{entity.Matricule}' existe deja.");

        if (entity.ModeRemuneration is not ("Forfait" or "Heures" or "Mixte"))
            throw new InvalidOperationException("Le mode de remuneration doit etre 'Forfait', 'Heures' ou 'Mixte'.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Enseignant {Name} cree avec l'id {Id}.", created.Name, created.Id);

        var evt = new EntityCreatedEvent<Enseignant>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.enseignant", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Enseignant entity, CancellationToken ct = default)
    {
        if (entity.ModeRemuneration is not ("Forfait" or "Heures" or "Mixte"))
            throw new InvalidOperationException("Le mode de remuneration doit etre 'Forfait', 'Heures' ou 'Mixte'.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Enseignant {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Enseignant>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.enseignant", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Enseignant {Id} supprime.", id);

            var evt = new EntityDeletedEvent<Enseignant>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.enseignant", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
