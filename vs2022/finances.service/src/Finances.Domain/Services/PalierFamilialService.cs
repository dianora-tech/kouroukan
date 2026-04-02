using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using Finances.Domain.Ports.Output;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace Finances.Domain.Services;

/// <summary>
/// Logique metier pour les paliers familiaux.
/// </summary>
public sealed class PalierFamilialService : IPalierFamilialService
{
    private const string Exchange = "kouroukan.events";
    private readonly IPalierFamilialRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<PalierFamilialService> _logger;

    public PalierFamilialService(
        IPalierFamilialRepository repository,
        IMessagePublisher publisher,
        ILogger<PalierFamilialService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<PalierFamilial?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<PalierFamilial>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<PalierFamilial>> GetPagedAsync(
        int page, int pageSize, int? companyId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, companyId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<PalierFamilial> CreateAsync(PalierFamilial entity, CancellationToken ct = default)
    {
        if (entity.RangEnfant <= 0)
            throw new InvalidOperationException("Le rang de l'enfant doit etre superieur a 0.");

        if (entity.ReductionPourcent < 0 || entity.ReductionPourcent > 100)
            throw new InvalidOperationException("Le pourcentage de reduction doit etre entre 0 et 100.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("PalierFamilial cree avec l'id {Id} pour l'etablissement {CompanyId} - rang {Rang}, reduction {Reduction}%.",
            created.Id, created.CompanyId, created.RangEnfant, created.ReductionPourcent);

        var evt = new EntityCreatedEvent<PalierFamilial>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.palier-familial", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("PalierFamilial {Id} supprime.", id);

            var evt = new EntityDeletedEvent<PalierFamilial>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.palier-familial", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
