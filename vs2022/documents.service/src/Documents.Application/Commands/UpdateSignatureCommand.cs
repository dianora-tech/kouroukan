using MediatR;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une signature electronique.
/// </summary>
public sealed record UpdateSignatureCommand(
    int Id,
    int TypeId,
    int DocumentGenereId,
    int SignataireId,
    int OrdreSignature,
    DateTime? DateSignature,
    string StatutSignature,
    string NiveauSignature,
    string? MotifRefus,
    bool EstValidee,
    int UserId) : IRequest<bool>;
