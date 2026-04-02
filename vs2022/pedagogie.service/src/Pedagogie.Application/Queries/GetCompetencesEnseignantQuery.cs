using GnDapper.Models;
using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des competences enseignant.
/// </summary>
public sealed record GetPagedCompetencesEnseignantQuery(
    int Page,
    int PageSize,
    int? UserId,
    string? CycleEtude,
    string? OrderBy) : IRequest<PagedResult<CompetenceEnseignant>>;
