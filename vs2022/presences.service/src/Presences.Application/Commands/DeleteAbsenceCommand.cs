using MediatR;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de suppression d'une absence.
/// </summary>
public sealed record DeleteAbsenceCommand(int Id) : IRequest<bool>;
