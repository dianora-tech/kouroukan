using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Support.Domain.Ports.Output;

namespace Support.Domain.Services;

/// <summary>
/// Service metier pour la gestion des articles d'aide.
/// </summary>
public sealed class ArticleAideService : IArticleAideService
{
    private const string Exchange = "kouroukan.events";
    private readonly IArticleAideRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<ArticleAideService> _logger;

    public ArticleAideService(
        IArticleAideRepository repository,
        IMessagePublisher publisher,
        ILogger<ArticleAideService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ArticleAide?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ArticleAide>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct);
    }

    /// <inheritdoc />
    public async Task<PagedResult<ArticleAide>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct);
    }

    /// <inheritdoc />
    public async Task<ArticleAide> CreateAsync(ArticleAide entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Titre))
            throw new InvalidOperationException("Le titre de l'article est obligatoire.");

        if (string.IsNullOrWhiteSpace(entity.Contenu))
            throw new InvalidOperationException("Le contenu de l'article est obligatoire.");

        if (string.IsNullOrWhiteSpace(entity.Slug))
            throw new InvalidOperationException("Le slug de l'article est obligatoire.");

        var existing = await _repository.GetBySlugAsync(entity.Slug, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Un article avec le slug '{entity.Slug}' existe deja.");

        entity.NombreVues = 0;
        entity.NombreUtile = 0;

        var created = await _repository.AddAsync(entity, ct);
        _logger.LogInformation("Article d'aide '{Titre}' cree avec l'id {Id}.", created.Titre, created.Id);

        var evt = new EntityCreatedEvent<ArticleAide>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.articleaide", cancellationToken: ct);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(ArticleAide entity, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(entity.Id, ct);
        if (existing is null)
            return false;

        if (!string.IsNullOrWhiteSpace(entity.Slug) && entity.Slug != existing.Slug)
        {
            var slugExists = await _repository.GetBySlugAsync(entity.Slug, ct);
            if (slugExists is not null)
                throw new InvalidOperationException($"Un article avec le slug '{entity.Slug}' existe deja.");
        }

        var result = await _repository.UpdateAsync(entity, ct);
        _logger.LogInformation("Article d'aide {Id} mis a jour.", entity.Id);

        var evt = new EntityUpdatedEvent<ArticleAide>(entity) { EntityId = entity.Id, UserId = entity.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.updated.articleaide", cancellationToken: ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct);
        if (result)
        {
            _logger.LogInformation("Article d'aide {Id} supprime.", id);
            var evt = new EntityDeletedEvent<ArticleAide> { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.articleaide", cancellationToken: ct);
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ArticleAide>> RechercherFullTextAsync(string query, int limit = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Array.Empty<ArticleAide>();

        return await _repository.RechercherFullTextAsync(query, limit, ct);
    }

    /// <inheritdoc />
    public async Task<bool> MarquerUtileAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            throw new KeyNotFoundException($"Article {id} introuvable.");

        return await _repository.IncrementUtileAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task<bool> IncrementerVuesAsync(int id, CancellationToken ct = default)
    {
        return await _repository.IncrementVuesAsync(id, ct);
    }
}
