using Finances.Application.Handlers;
using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using GnDapper.Models;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour PaiementQueryHandler.
/// </summary>
public sealed class PaiementQueryHandlerTests
{
    private readonly Mock<IPaiementService> _serviceMock;
    private readonly PaiementQueryHandler _sut;

    public PaiementQueryHandlerTests()
    {
        _serviceMock = new Mock<IPaiementService>();
        _sut = new PaiementQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetournePaiement()
    {
        var entity = new Paiement { Id = 1, NumeroRecu = "REC-001" };
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var result = await _sut.Handle(new GetPaiementByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var list = new List<Paiement> { new() { Id = 1 }, new() { Id = 2 } };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = await _sut.Handle(new GetAllPaiementsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Paiement>(new List<Paiement> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock.Setup(s => s.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var result = await _sut.Handle(new GetPagedPaiementsQuery(1, 20, null, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}
