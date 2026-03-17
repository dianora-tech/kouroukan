using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les documents generes.
/// </summary>
public sealed record GetAllDocumentGeneresQuery() : IRequest<IReadOnlyList<DocumentGenere>>;
