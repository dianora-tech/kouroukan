using MediatR;

namespace Personnel.Application.Commands;

/// <summary>
/// Commande de suppression d'une demande de conge.
/// </summary>
public sealed record DeleteDemandeCongeCommand(int Id) : IRequest<bool>;
