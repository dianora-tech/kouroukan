using FluentAssertions;
using GnDapper.Models;
using Personnel.Application.Handlers;
using Personnel.Application.Queries;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Personnel.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour EnseignantQueryHandler.
/// </summary>
public sealed class EnseignantQueryHandlerTests
{
    private readonly Mock<IEnseignantService> _serviceMock;
    private readonly EnseignantQueryHandler _sut;

    public EnseignantQueryHandlerTests()
    {
        _serviceMock = new Mock<IEnseignantService>();
        _sut = new EnseignantQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneEnseignant()
    {
        var enseignant = new Enseignant { Id = 1, Name = "Mamadou Diallo", StatutEnseignant = "Actif" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enseignant);

        var result = await _sut.Handle(new GetEnseignantByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enseignant?)null);

        var result = await _sut.Handle(new GetEnseignantByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var enseignants = new List<Enseignant>
        {
            new() { Id = 1, Name = "Mamadou Diallo", StatutEnseignant = "Actif" },
            new() { Id = 2, Name = "Fatoumata Camara", StatutEnseignant = "EnConge" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(enseignants);

        var result = await _sut.Handle(new GetAllEnseignantsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListeVide_SiAucunEnseignant()
    {
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Enseignant>());

        var result = await _sut.Handle(new GetAllEnseignantsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Enseignant>(
            new List<Enseignant> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedEnseignantsQuery(1, 20, "test", null, "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPaged_TransmetTousLesParametres()
    {
        var paged = new PagedResult<Enseignant>(
            new List<Enseignant>(), 0, 2, 10);
        _serviceMock
            .Setup(s => s.GetPagedAsync(2, 10, "math", 5, "name", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedEnseignantsQuery(2, 10, "math", 5, "name"),
            CancellationToken.None);

        result.TotalCount.Should().Be(0);
        _serviceMock.Verify(s => s.GetPagedAsync(2, 10, "math", 5, "name", It.IsAny<CancellationToken>()), Times.Once);
    }
}
