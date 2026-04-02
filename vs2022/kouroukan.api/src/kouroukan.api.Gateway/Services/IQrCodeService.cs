using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service de gestion des QR codes utilisateur.
/// </summary>
public interface IQrCodeService
{
    /// <summary>
    /// Recupere le QR code de l'utilisateur. Le cree s'il n'existe pas.
    /// </summary>
    Task<QrCodeDto> GetOrCreateQrCodeAsync(int userId, CancellationToken ct = default);

    /// <summary>
    /// Resout un QR code vers un profil utilisateur.
    /// </summary>
    Task<QrCodeResolvedDto?> ResolveQrCodeAsync(string code, CancellationToken ct = default);
}
