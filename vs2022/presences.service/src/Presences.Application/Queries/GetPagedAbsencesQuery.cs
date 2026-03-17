using GnDapper.Models;
using MediatR;
using AbsenceEntity = Presences.Domain.Entities.Absence;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des absences.
/// </summary>
public sealed record GetPagedAbsencesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<AbsenceEntity>>;
