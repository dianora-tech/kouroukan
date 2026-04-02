using MediatR;

namespace Finances.Application.Commands;

/// <summary>
/// Commande de suppression d'un palier familial.
/// </summary>
public sealed record DeletePalierFamilialCommand(int Id) : IRequest<bool>;
