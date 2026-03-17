using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Evaluations.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Evaluations.Domain.Services;

/// <summary>
/// Service metier pour les evaluations.
/// </summary>
public sealed class EvaluationService : IEvaluationService
{
    private const string Exchange = "kouroukan.events";

    private readonly IEvaluationRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<EvaluationService> _logger;

    public EvaluationService(
        IEvaluationRepository repository,
        IMessagePublisher publisher,
        ILogger<EvaluationService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Evaluation?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Evaluation>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Evaluation>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Evaluation> CreateAsync(Evaluation entity, CancellationToken ct = default)
    {
        if (entity.Trimestre is < 1 or > 3)
            throw new InvalidOperationException(
                $"Le trimestre doit etre compris entre 1 et 3. Valeur recue : {entity.Trimestre}.");

        if (entity.NoteMaximale <= 0)
            throw new InvalidOperationException("La note maximale doit etre superieure a 0.");

        if (entity.Coefficient <= 0)
            throw new InvalidOperationException("Le coefficient doit etre superieur a 0.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Evaluation {Id} creee pour la matiere {MatiereId} dans la classe {ClasseId}.",
            created.Id, created.MatiereId, created.ClasseId);

        var evt = new EntityCreatedEvent<Evaluation>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.evaluation", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Evaluation entity, CancellationToken ct = default)
    {
        if (entity.Trimestre is < 1 or > 3)
            throw new InvalidOperationException(
                $"Le trimestre doit etre compris entre 1 et 3. Valeur recue : {entity.Trimestre}.");

        if (entity.NoteMaximale <= 0)
            throw new InvalidOperationException("La note maximale doit etre superieure a 0.");

        if (entity.Coefficient <= 0)
            throw new InvalidOperationException("Le coefficient doit etre superieur a 0.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Evaluation {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Evaluation>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.evaluation", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Evaluation {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Evaluation>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.evaluation", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
