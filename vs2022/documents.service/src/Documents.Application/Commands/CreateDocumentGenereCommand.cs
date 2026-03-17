using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de creation d'un document genere.
/// </summary>
public sealed record CreateDocumentGenereCommand(
    int TypeId,
    int ModeleDocumentId,
    int? EleveId,
    int? EnseignantId,
    string DonneesJson,
    DateTime DateGeneration,
    string StatutSignature,
    string? CheminFichier,
    int UserId) : IRequest<DocumentGenere>;
