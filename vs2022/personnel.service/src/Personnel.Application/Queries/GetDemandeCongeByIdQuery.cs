using Personnel.Domain.Entities;
using MediatR;

namespace Personnel.Application.Queries;

/// <summary>
/// Requete de recuperation d'une demande de conge par son identifiant.
/// </summary>
public sealed record GetDemandeCongeByIdQuery(int Id) : IRequest<DemandeConge?>;
