using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande d'initiation d'un transfert d'eleve.
/// </summary>
public sealed record CreateTransfertCommand(
    int EleveId,
    int CompanyOrigineId,
    int CompanyCibleId,
    string Motif,
    string? Documents) : IRequest<Transfert>;
