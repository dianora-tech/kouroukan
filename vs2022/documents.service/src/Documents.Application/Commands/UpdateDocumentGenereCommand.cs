using MediatR;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un document genere.
/// </summary>
public sealed record UpdateDocumentGenereCommand(
    int Id,
    int TypeId,
    int ModeleDocumentId,
    int? EleveId,
    int? EnseignantId,
    string DonneesJson,
    DateTime DateGeneration,
    string StatutSignature,
    string? CheminFichier,
    int UserId) : IRequest<bool>;
