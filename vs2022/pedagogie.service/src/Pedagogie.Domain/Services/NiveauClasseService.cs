using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Pedagogie.Domain.Services;

/// <summary>
/// Service metier pour les niveaux de classes.
/// </summary>
public sealed class NiveauClasseService : INiveauClasseService
{
    private const string Exchange = "kouroukan.events";
    private readonly INiveauClasseRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<NiveauClasseService> _logger;

    public NiveauClasseService(
        INiveauClasseRepository repository,
        IMessagePublisher publisher,
        ILogger<NiveauClasseService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<NiveauClasse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<NiveauClasse>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<NiveauClasse>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<NiveauClasse> CreateAsync(NiveauClasse entity, CancellationToken ct = default)
    {
        if (entity.Ordre < 1)
            throw new InvalidOperationException("L'ordre du niveau doit etre superieur a 0.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Niveau de classe {Code} cree avec l'id {Id}.", created.Code, created.Id);

        var evt = new EntityCreatedEvent<NiveauClasse>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.niveauclasse", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(NiveauClasse entity, CancellationToken ct = default)
    {
        if (entity.Ordre < 1)
            throw new InvalidOperationException("L'ordre du niveau doit etre superieur a 0.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Niveau de classe {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<NiveauClasse>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.niveauclasse", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Niveau de classe {Id} supprime.", id);

            var evt = new EntityDeletedEvent<NiveauClasse>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.niveauclasse", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
