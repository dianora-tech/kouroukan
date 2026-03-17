using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Pedagogie.Domain.Services;

/// <summary>
/// Service metier pour les classes.
/// </summary>
public sealed class ClasseService : IClasseService
{
    private const string Exchange = "kouroukan.events";
    private readonly IClasseRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<ClasseService> _logger;

    public ClasseService(
        IClasseRepository repository,
        IMessagePublisher publisher,
        ILogger<ClasseService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Classe?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Classe>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Classe>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Classe> CreateAsync(Classe entity, CancellationToken ct = default)
    {
        if (entity.Capacite <= 0)
            throw new InvalidOperationException("La capacite de la classe doit etre superieure a 0.");

        if (entity.Effectif > entity.Capacite)
            throw new InvalidOperationException("L'effectif ne peut pas depasser la capacite de la classe.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Classe {Name} creee avec l'id {Id}.", created.Name, created.Id);

        var evt = new EntityCreatedEvent<Classe>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.classe", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Classe entity, CancellationToken ct = default)
    {
        if (entity.Capacite <= 0)
            throw new InvalidOperationException("La capacite de la classe doit etre superieure a 0.");

        if (entity.Effectif > entity.Capacite)
            throw new InvalidOperationException("L'effectif ne peut pas depasser la capacite de la classe.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Classe {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Classe>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.classe", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Classe {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Classe>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.classe", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
