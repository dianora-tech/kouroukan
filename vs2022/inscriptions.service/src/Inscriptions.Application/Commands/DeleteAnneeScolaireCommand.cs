using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de suppression d'une annee scolaire.
/// </summary>
public sealed record DeleteAnneeScolaireCommand(int Id) : IRequest<bool>;
