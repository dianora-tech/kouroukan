using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des moyens de paiement.
/// </summary>
public sealed record GetPagedMoyensPaiementQuery(
    int Page,
    int PageSize,
    int? CompanyId,
    string? OrderBy) : IRequest<PagedResult<MoyenPaiement>>;
