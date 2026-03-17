using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les modeles de documents.
/// </summary>
public sealed record GetAllModeleDocumentsQuery() : IRequest<IReadOnlyList<ModeleDocument>>;
