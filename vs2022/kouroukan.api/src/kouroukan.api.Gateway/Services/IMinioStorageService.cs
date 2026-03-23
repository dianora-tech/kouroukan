namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service de stockage de fichiers via MinIO (S3-compatible).
/// </summary>
public interface IMinioStorageService
{
    /// <summary>
    /// Upload un avatar utilisateur et retourne l'URL publique.
    /// </summary>
    Task<string> UploadAvatarAsync(int userId, Stream fileStream, string contentType, string fileName, CancellationToken ct = default);
}
