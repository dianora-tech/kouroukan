using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les notes.
/// </summary>
public sealed record GetAllNotesQuery() : IRequest<IReadOnlyList<Note>>;
