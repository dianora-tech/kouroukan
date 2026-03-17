using GnDapper.Models;
using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des dossiers d'admission.
/// </summary>
public sealed record GetPagedDossierAdmissionsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<DossierAdmission>>;
