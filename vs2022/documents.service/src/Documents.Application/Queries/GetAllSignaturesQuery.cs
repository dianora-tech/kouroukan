using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les signatures.
/// </summary>
public sealed record GetAllSignaturesQuery() : IRequest<IReadOnlyList<Signature>>;
