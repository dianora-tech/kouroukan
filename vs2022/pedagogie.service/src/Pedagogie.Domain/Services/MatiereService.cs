using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Pedagogie.Domain.Services;

/// <summary>
/// Service metier pour les matieres.
/// </summary>
public sealed class MatiereService : IMatiereService
{
    private const string Exchange = "kouroukan.events";
    private readonly IMatiereRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<MatiereService> _logger;

    public MatiereService(
        IMatiereRepository repository,
        IMessagePublisher publisher,
        ILogger<MatiereService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Matiere?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Matiere>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Matiere>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Matiere> CreateAsync(Matiere entity, CancellationToken ct = default)
    {
        if (entity.Coefficient <= 0)
            throw new InvalidOperationException("Le coefficient de la matiere doit etre superieur a 0.");

        if (entity.NombreHeures <= 0)
            throw new InvalidOperationException("Le nombre d'heures doit etre superieur a 0.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Matiere {Code} creee avec l'id {Id}.", created.Code, created.Id);

        var evt = new EntityCreatedEvent<Matiere>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.matiere", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Matiere entity, CancellationToken ct = default)
    {
        if (entity.Coefficient <= 0)
            throw new InvalidOperationException("Le coefficient de la matiere doit etre superieur a 0.");

        if (entity.NombreHeures <= 0)
            throw new InvalidOperationException("Le nombre d'heures doit etre superieur a 0.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Matiere {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Matiere>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.matiere", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Matiere {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Matiere>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.matiere", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
