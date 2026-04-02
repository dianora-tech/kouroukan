using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Pedagogie.Domain.Services;

/// <summary>
/// Service metier pour les affectations enseignant.
/// </summary>
public sealed class AffectationEnseignantService : IAffectationEnseignantService
{
    private const string Exchange = "kouroukan.events";
    private readonly IAffectationEnseignantRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<AffectationEnseignantService> _logger;

    public AffectationEnseignantService(
        IAffectationEnseignantRepository repository,
        IMessagePublisher publisher,
        ILogger<AffectationEnseignantService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<AffectationEnseignant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AffectationEnseignant>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<AffectationEnseignant>> GetPagedAsync(
        int page, int pageSize, int? liaisonId, int? classeId, int? matiereId, int? anneeScolaireId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, liaisonId, classeId, matiereId, anneeScolaireId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<AffectationEnseignant> CreateAsync(AffectationEnseignant entity, CancellationToken ct = default)
    {
        entity.EstActive = true;

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("AffectationEnseignant creee avec l'id {Id} pour la liaison {LiaisonId}, classe {ClasseId}.",
            created.Id, created.LiaisonId, created.ClasseId);

        var evt = new EntityCreatedEvent<AffectationEnseignant>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.affectation-enseignant", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(AffectationEnseignant entity, CancellationToken ct = default)
    {
        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("AffectationEnseignant {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<AffectationEnseignant>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.affectation-enseignant", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("AffectationEnseignant {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<AffectationEnseignant>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.affectation-enseignant", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
