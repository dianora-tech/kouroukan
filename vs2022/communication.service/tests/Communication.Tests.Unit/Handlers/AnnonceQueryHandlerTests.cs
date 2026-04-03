using FluentAssertions;
using GnDapper.Models;
using Communication.Application.Handlers;
using Communication.Application.Queries;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Communication.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AnnonceQueryHandler.
/// </summary>
public sealed class AnnonceQueryHandlerTests
{
    private readonly Mock<IAnnonceService> _serviceMock;
    private readonly AnnonceQueryHandler _sut;

    public AnnonceQueryHandlerTests()
    {
        _serviceMock = new Mock<IAnnonceService>();
        _sut = new AnnonceQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneAnnonce()
    {
        var annonce = new Annonce { Id = 1, Contenu = "Test" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(annonce);

        var result = await _sut.Handle(new GetAnnonceByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Annonce?)null);

        var result = await _sut.Handle(new GetAnnonceByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var annonces = new List<Annonce>
        {
            new() { Id = 1, Contenu = "Test 1" },
            new() { Id = 2, Contenu = "Test 2" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(annonces);

        var result = await _sut.Handle(new GetAllAnnoncesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Annonce>(
            new List<Annonce> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedAnnoncesQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
