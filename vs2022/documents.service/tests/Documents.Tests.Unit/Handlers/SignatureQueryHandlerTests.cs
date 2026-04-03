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
/// Tests unitaires pour SignatureQueryHandler.
/// </summary>
public sealed class SignatureQueryHandlerTests
{
    private readonly Mock<ISignatureService> _serviceMock;
    private readonly SignatureQueryHandler _sut;

    public SignatureQueryHandlerTests()
    {
        _serviceMock = new Mock<ISignatureService>();
        _sut = new SignatureQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneSignature()
    {
        var entity = new Signature { Id = 1, StatutSignature = "EnAttente" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.Handle(new GetSignatureByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Signature?)null);

        var result = await _sut.Handle(new GetSignatureByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var entities = new List<Signature>
        {
            new() { Id = 1, StatutSignature = "EnAttente" },
            new() { Id = 2, StatutSignature = "Signe" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _sut.Handle(new GetAllSignaturesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Signature>(
            new List<Signature> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedSignaturesQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
