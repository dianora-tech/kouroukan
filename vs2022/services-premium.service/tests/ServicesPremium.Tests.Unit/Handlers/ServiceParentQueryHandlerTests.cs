using FluentAssertions;
using GnDapper.Models;
using Moq;
using ServicesPremium.Application.Handlers;
using ServicesPremium.Application.Queries;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;
using Xunit;

namespace ServicesPremium.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour ServiceParentQueryHandler.
/// </summary>
public sealed class ServiceParentQueryHandlerTests
{
    private readonly Mock<IServiceParentService> _serviceMock;
    private readonly ServiceParentQueryHandler _sut;

    public ServiceParentQueryHandlerTests()
    {
        _serviceMock = new Mock<IServiceParentService>();
        _sut = new ServiceParentQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneServiceParent()
    {
        var entity = new ServiceParent { Id = 1, Code = "SVC-001", Periodicite = "Mensuel" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.Handle(new GetServiceParentByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ServiceParent?)null);

        var result = await _sut.Handle(new GetServiceParentByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var list = new List<ServiceParent>
        {
            new() { Id = 1, Code = "SVC-001" },
            new() { Id = 2, Code = "SVC-002" }
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _sut.Handle(new GetAllServiceParentsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<ServiceParent>(
            new List<ServiceParent> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedServiceParentsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
