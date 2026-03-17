using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation d'un modele de document par son identifiant.
/// </summary>
public sealed record GetModeleDocumentByIdQuery(int Id) : IRequest<ModeleDocument?>;
