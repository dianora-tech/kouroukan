using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Bde.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Bde.Domain.Services;

/// <summary>
/// Service metier pour les associations.
/// </summary>
public sealed class AssociationService : IAssociationService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] StatutsValides = ["Active", "Suspendue", "Dissoute"];

    private readonly IAssociationRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<AssociationService> _logger;

    public AssociationService(
        IAssociationRepository repository,
        IMessagePublisher publisher,
        ILogger<AssociationService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Association?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Association>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Association>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Association> CreateAsync(Association entity, CancellationToken ct = default)
    {
        if (!StatutsValides.Contains(entity.Statut))
            throw new InvalidOperationException(
                $"Statut d'association invalide : '{entity.Statut}'.");

        if (entity.BudgetAnnuel < 0)
            throw new InvalidOperationException("Le budget annuel ne peut pas etre negatif.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Association {Id} creee avec le nom {Name}.",
            created.Id, created.Name);

        var evt = new EntityCreatedEvent<Association>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.association", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Association entity, CancellationToken ct = default)
    {
        if (!StatutsValides.Contains(entity.Statut))
            throw new InvalidOperationException(
                $"Statut d'association invalide : '{entity.Statut}'.");

        if (entity.BudgetAnnuel < 0)
            throw new InvalidOperationException("Le budget annuel ne peut pas etre negatif.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Association {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Association>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.association", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Association {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Association>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.association", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
