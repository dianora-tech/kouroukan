using FluentAssertions;
using GnDapper.Models;
using Documents.Application.Handlers;
using Documents.Application.Queries;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Documents.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour DocumentGenereQueryHandler.
/// </summary>
public sealed class DocumentGenereQueryHandlerTests
{
    private readonly Mock<IDocumentGenereService> _serviceMock;
    private readonly DocumentGenereQueryHandler _sut;

    public DocumentGenereQueryHandlerTests()
    {
        _serviceMock = new Mock<IDocumentGenereService>();
        _sut = new DocumentGenereQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneDocumentGenere()
    {
        var entity = new DocumentGenere { Id = 1, StatutSignature = "EnAttente" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.Handle(new GetDocumentGenereByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DocumentGenere?)null);

        var result = await _sut.Handle(new GetDocumentGenereByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var entities = new List<DocumentGenere>
        {
            new() { Id = 1, StatutSignature = "EnAttente" },
            new() { Id = 2, StatutSignature = "Signe" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _sut.Handle(new GetAllDocumentGeneresQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<DocumentGenere>(
            new List<DocumentGenere> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedDocumentGeneresQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
