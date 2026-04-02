using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des paliers familiaux.
/// </summary>
public sealed record GetPagedPaliersFamiliauxQuery(
    int Page,
    int PageSize,
    int? CompanyId,
    string? OrderBy) : IRequest<PagedResult<PalierFamilial>>;
