using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Pedagogie.Domain.Services;

/// <summary>
/// Service metier pour les competences enseignant.
/// </summary>
public sealed class CompetenceEnseignantService : ICompetenceEnseignantService
{
    private const string Exchange = "kouroukan.events";
    private readonly ICompetenceEnseignantRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<CompetenceEnseignantService> _logger;

    public CompetenceEnseignantService(
        ICompetenceEnseignantRepository repository,
        IMessagePublisher publisher,
        ILogger<CompetenceEnseignantService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<CompetenceEnseignant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<CompetenceEnseignant>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<CompetenceEnseignant>> GetPagedAsync(
        int page, int pageSize, int? userId, string? cycleEtude, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, userId, cycleEtude, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<CompetenceEnseignant> CreateAsync(CompetenceEnseignant entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.CycleEtude))
            throw new InvalidOperationException("Le cycle d'etude est obligatoire.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("CompetenceEnseignant creee avec l'id {Id} pour l'enseignant {UserId}.", created.Id, created.UserId);

        var evt = new EntityCreatedEvent<CompetenceEnseignant>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.competence-enseignant", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("CompetenceEnseignant {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<CompetenceEnseignant>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.competence-enseignant", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
