using FluentAssertions;
using GnDapper.Models;
using Inscriptions.Application.Handlers;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour InscriptionQueryHandler.
/// </summary>
public sealed class InscriptionQueryHandlerTests
{
    private readonly Mock<IInscriptionService> _serviceMock;
    private readonly InscriptionQueryHandler _sut;

    public InscriptionQueryHandlerTests()
    {
        _serviceMock = new Mock<IInscriptionService>();
        _sut = new InscriptionQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneInscription()
    {
        var inscription = new Inscription { Id = 1, StatutInscription = "EnAttente" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscription);

        var result = await _sut.Handle(new GetInscriptionByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inscription?)null);

        var result = await _sut.Handle(new GetInscriptionByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var inscriptions = new List<Inscription>
        {
            new() { Id = 1, StatutInscription = "EnAttente" },
            new() { Id = 2, StatutInscription = "Validee" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscriptions);

        var result = await _sut.Handle(new GetAllInscriptionsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Inscription>(
            new List<Inscription> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedInscriptionsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
