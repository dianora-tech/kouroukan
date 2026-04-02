using GnDapper.Models;
using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des affectations enseignant.
/// </summary>
public sealed record GetPagedAffectationsEnseignantQuery(
    int Page,
    int PageSize,
    int? LiaisonId,
    int? ClasseId,
    int? MatiereId,
    int? AnneeScolaireId,
    string? OrderBy) : IRequest<PagedResult<AffectationEnseignant>>;
