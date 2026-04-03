using FluentAssertions;
using Kouroukan.Api.Gateway.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Services;

public sealed class MinioStorageServiceTests
{
    private readonly Mock<IMinioClient> _minioClientMock;
    private readonly Mock<ILogger<MinioStorageService>> _loggerMock;
    private readonly IConfiguration _configuration;
    private readonly MinioStorageService _sut;

    public MinioStorageServiceTests()
    {
        _minioClientMock = new Mock<IMinioClient>();
        _loggerMock = new Mock<ILogger<MinioStorageService>>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MinIO:PublicEndpoint"] = "https://minio.test.com/avatars"
            })
            .Build();
        _sut = new MinioStorageService(_minioClientMock.Object, _loggerMock.Object, _configuration);
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependancesValides()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitImplementerIMinioStorageService()
    {
        // Assert
        _sut.Should().BeAssignableTo<IMinioStorageService>();
    }

    [Fact]
    public void Constructeur_DoitUtiliserEndpointParDefaut_QuandConfigurationAbsente()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();

        // Act
        var service = new MinioStorageService(_minioClientMock.Object, _loggerMock.Object, emptyConfig);

        // Assert
        service.Should().NotBeNull();
    }

    // ─── UploadAvatarAsync ───

    [Fact]
    public async Task UploadAvatarAsync_DoitCreerBucket_QuandInexistant()
    {
        // Arrange
        _minioClientMock
            .Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _minioClientMock
            .Setup(x => x.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _minioClientMock
            .Setup(x => x.SetPolicyAsync(It.IsAny<SetPolicyArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _minioClientMock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(default(Minio.DataModel.Response.PutObjectResponse)!));

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        // Act
        var url = await _sut.UploadAvatarAsync(42, stream, "image/png", "avatar.png");

        // Assert
        url.Should().StartWith("https://minio.test.com/avatars/42/");
        url.Should().EndWith(".png");
        _minioClientMock.Verify(x => x.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadAvatarAsync_DoitNePasCreerBucket_QuandExisteDeja()
    {
        // Arrange
        _minioClientMock
            .Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _minioClientMock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(default(Minio.DataModel.Response.PutObjectResponse)!));

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        // Act
        var url = await _sut.UploadAvatarAsync(42, stream, "image/png", "avatar.png");

        // Assert
        url.Should().Contain("42/");
        _minioClientMock.Verify(x => x.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadAvatarAsync_DoitRetournerUrlAvecExtension_QuandJpeg()
    {
        // Arrange
        _minioClientMock
            .Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _minioClientMock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(default(Minio.DataModel.Response.PutObjectResponse)!));

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        // Act
        var url = await _sut.UploadAvatarAsync(10, stream, "image/jpeg", "photo.JPEG");

        // Assert
        url.Should().EndWith(".jpeg");
    }

    [Fact]
    public async Task UploadAvatarAsync_DoitAppelerPutObject()
    {
        // Arrange
        _minioClientMock
            .Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _minioClientMock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(default(Minio.DataModel.Response.PutObjectResponse)!));

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        // Act
        await _sut.UploadAvatarAsync(42, stream, "image/png", "avatar.png");

        // Assert
        _minioClientMock.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
