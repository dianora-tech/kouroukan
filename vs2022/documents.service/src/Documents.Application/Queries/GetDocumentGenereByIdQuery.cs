using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation d'un document genere par son identifiant.
/// </summary>
public sealed record GetDocumentGenereByIdQuery(int Id) : IRequest<DocumentGenere?>;
