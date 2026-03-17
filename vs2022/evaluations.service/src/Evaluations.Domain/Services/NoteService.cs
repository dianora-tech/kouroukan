using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Evaluations.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Evaluations.Domain.Services;

/// <summary>
/// Service metier pour les notes.
/// </summary>
public sealed class NoteService : INoteService
{
    private const string Exchange = "kouroukan.events";

    private readonly INoteRepository _repository;
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<NoteService> _logger;

    public NoteService(
        INoteRepository repository,
        IEvaluationRepository evaluationRepository,
        IMessagePublisher publisher,
        ILogger<NoteService> logger)
    {
        _repository = repository;
        _evaluationRepository = evaluationRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Note?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Note>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Note> CreateAsync(Note entity, CancellationToken ct = default)
    {
        var evaluation = await _evaluationRepository.GetByIdAsync(entity.EvaluationId, ct).ConfigureAwait(false);
        if (evaluation is null)
            throw new KeyNotFoundException($"L'evaluation avec l'id {entity.EvaluationId} n'existe pas.");

        if (entity.Valeur < 0)
            throw new InvalidOperationException("La note ne peut pas etre negative.");

        if (entity.Valeur > evaluation.NoteMaximale)
            throw new InvalidOperationException(
                $"La note ({entity.Valeur}) ne peut pas depasser la note maximale ({evaluation.NoteMaximale}).");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Note {Id} creee pour l'eleve {EleveId} a l'evaluation {EvaluationId}.",
            created.Id, created.EleveId, created.EvaluationId);

        var evt = new EntityCreatedEvent<Note>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.note", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Note entity, CancellationToken ct = default)
    {
        var evaluation = await _evaluationRepository.GetByIdAsync(entity.EvaluationId, ct).ConfigureAwait(false);
        if (evaluation is null)
            throw new KeyNotFoundException($"L'evaluation avec l'id {entity.EvaluationId} n'existe pas.");

        if (entity.Valeur < 0)
            throw new InvalidOperationException("La note ne peut pas etre negative.");

        if (entity.Valeur > evaluation.NoteMaximale)
            throw new InvalidOperationException(
                $"La note ({entity.Valeur}) ne peut pas depasser la note maximale ({evaluation.NoteMaximale}).");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Note {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Note>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.note", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Note {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Note>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.note", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
