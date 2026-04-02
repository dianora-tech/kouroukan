using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Queries;

/// <summary>
/// Requete de recuperation paginee du journal financier.
/// </summary>
public sealed record GetPagedJournalFinancierQuery(
    int Page,
    int PageSize,
    int? CompanyId,
    string? Type,
    string? Categorie,
    DateTime? DateDebut,
    DateTime? DateFin,
    string? OrderBy) : IRequest<PagedResult<JournalFinancier>>;
