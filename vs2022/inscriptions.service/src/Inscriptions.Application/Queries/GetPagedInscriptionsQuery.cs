using GnDapper.Models;
using MediatR;
using InscriptionEntity = Inscriptions.Domain.Entities.Inscription;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des inscriptions.
/// </summary>
public sealed record GetPagedInscriptionsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<InscriptionEntity>>;
