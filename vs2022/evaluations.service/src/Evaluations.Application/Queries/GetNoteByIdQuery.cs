using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation d'une note par son identifiant.
/// </summary>
public sealed record GetNoteByIdQuery(int Id) : IRequest<Note?>;
