using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de creation d'une signature electronique.
/// </summary>
public sealed record CreateSignatureCommand(
    int TypeId,
    int DocumentGenereId,
    int SignataireId,
    int OrdreSignature,
    DateTime? DateSignature,
    string StatutSignature,
    string NiveauSignature,
    string? MotifRefus,
    bool EstValidee,
    int UserId) : IRequest<Signature>;
