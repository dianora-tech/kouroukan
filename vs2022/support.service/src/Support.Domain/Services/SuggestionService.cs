using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Support.Domain.Ports.Output;

namespace Support.Domain.Services;

/// <summary>
/// Service metier pour la gestion des suggestions.
/// </summary>
public sealed class SuggestionService : ISuggestionService
{
    private const string Exchange = "kouroukan.events";
    private readonly ISuggestionRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<SuggestionService> _logger;

    public SuggestionService(
        ISuggestionRepository repository,
        IMessagePublisher publisher,
        ILogger<SuggestionService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Suggestion?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Suggestion>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Suggestion>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct);
    }

    /// <inheritdoc />
    public async Task<Suggestion> CreateAsync(Suggestion entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Titre))
            throw new InvalidOperationException("Le titre de la suggestion est obligatoire.");

        if (string.IsNullOrWhiteSpace(entity.Contenu))
            throw new InvalidOperationException("Le contenu de la suggestion est obligatoire.");

        entity.StatutSuggestion = "Soumise";
        entity.NombreVotes = 0;

        var created = await _repository.AddAsync(entity, ct);
        _logger.LogInformation("Suggestion '{Titre}' creee avec l'id {Id}.", created.Titre, created.Id);

        var evt = new EntityCreatedEvent<Suggestion>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.suggestion", cancellationToken: ct);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Suggestion entity, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(entity.Id, ct);
        if (existing is null)
            return false;

        if (entity.StatutSuggestion is not ("Soumise" or "EnRevue" or "Acceptee" or "Planifiee" or "Realisee" or "Rejetee"))
            throw new InvalidOperationException("Le statut de la suggestion est invalide.");

        var result = await _repository.UpdateAsync(entity, ct);
        _logger.LogInformation("Suggestion {Id} mise a jour (statut: {Statut}).", entity.Id, entity.StatutSuggestion);

        var evt = new EntityUpdatedEvent<Suggestion>(entity) { EntityId = entity.Id, UserId = entity.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.updated.suggestion", cancellationToken: ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct);
        if (result)
        {
            _logger.LogInformation("Suggestion {Id} supprimee.", id);
            var evt = new EntityDeletedEvent<Suggestion> { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.suggestion", cancellationToken: ct);
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<bool> VoterAsync(int suggestionId, int votantId, int userId, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(suggestionId, ct))
            throw new KeyNotFoundException($"Suggestion {suggestionId} introuvable.");

        var existingVote = await _repository.GetVoteAsync(suggestionId, votantId, ct);
        if (existingVote is not null)
            throw new InvalidOperationException("Vous avez deja vote pour cette suggestion.");

        var vote = new VoteSuggestion
        {
            SuggestionId = suggestionId,
            VotantId = votantId,
            UserId = userId
        };

        await _repository.AddVoteAsync(vote, ct);
        await _repository.IncrementVotesAsync(suggestionId, ct);

        _logger.LogInformation("Vote ajoute pour la suggestion {SuggestionId} par l'utilisateur {VotantId}.", suggestionId, votantId);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> RetirerVoteAsync(int suggestionId, int votantId, CancellationToken ct = default)
    {
        var existingVote = await _repository.GetVoteAsync(suggestionId, votantId, ct);
        if (existingVote is null)
            throw new InvalidOperationException("Vous n'avez pas vote pour cette suggestion.");

        await _repository.DeleteVoteAsync(suggestionId, votantId, ct);
        await _repository.DecrementVotesAsync(suggestionId, ct);

        _logger.LogInformation("Vote retire pour la suggestion {SuggestionId} par l'utilisateur {VotantId}.", suggestionId, votantId);
        return true;
    }
}
