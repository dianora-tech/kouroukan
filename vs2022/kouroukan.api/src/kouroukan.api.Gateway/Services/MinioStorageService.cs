using Minio;
using Minio.DataModel.Args;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Implementation MinIO du service de stockage de fichiers.
/// </summary>
public sealed class MinioStorageService : IMinioStorageService
{
    private const string BucketName = "avatars";
    private readonly IMinioClient _client;
    private readonly ILogger<MinioStorageService> _logger;
    private readonly string _publicEndpoint;
    private bool _bucketEnsured;

    public MinioStorageService(IMinioClient client, ILogger<MinioStorageService> logger, IConfiguration configuration)
    {
        _client = client;
        _logger = logger;
        // URL publique accessible depuis le navigateur (via nginx proxy)
        _publicEndpoint = configuration["MinIO:PublicEndpoint"] ?? "/avatars";
    }

    public async Task<string> UploadAvatarAsync(int userId, Stream fileStream, string contentType, string fileName, CancellationToken ct = default)
    {
        await EnsureBucketExistsAsync(ct);

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var objectName = $"{userId}/{Guid.NewGuid()}{extension}";

        var putArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _client.PutObjectAsync(putArgs, ct);

        _logger.LogInformation("Avatar uploade pour l'utilisateur {UserId}: {ObjectName}", userId, objectName);

        return $"{_publicEndpoint}/{objectName}";
    }

    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        if (_bucketEnsured) return;

        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName), ct);
        if (!exists)
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(BucketName), ct);
            _logger.LogInformation("Bucket '{BucketName}' cree dans MinIO", BucketName);

            // Rendre le bucket public en lecture (pour servir les avatars)
            var policy = $$"""
            {
                "Version": "2012-10-17",
                "Statement": [{
                    "Effect": "Allow",
                    "Principal": {"AWS": ["*"]},
                    "Action": ["s3:GetObject"],
                    "Resource": ["arn:aws:s3:::{{BucketName}}/*"]
                }]
            }
            """;
            await _client.SetPolicyAsync(new SetPolicyArgs().WithBucket(BucketName).WithPolicy(policy), ct);
        }

        _bucketEnsured = true;
    }
}
