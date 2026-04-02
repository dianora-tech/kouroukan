using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de creation d'une liaison parent (scan QR par l'etablissement).
/// </summary>
public sealed record CreateLiaisonParentCommand(
    int ParentUserId,
    int EleveId,
    int CompanyId) : IRequest<LiaisonParent>;
