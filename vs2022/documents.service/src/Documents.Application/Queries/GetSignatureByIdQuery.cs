using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation d'une signature par son identifiant.
/// </summary>
public sealed record GetSignatureByIdQuery(int Id) : IRequest<Signature?>;
