using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de suppression d'un dossier d'admission.
/// </summary>
public sealed record DeleteDossierAdmissionCommand(int Id) : IRequest<bool>;
