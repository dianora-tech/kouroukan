using FluentAssertions;
using GnDapper.Models;
using Presences.Application.Handlers;
using Presences.Application.Queries;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Presences.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AbsenceQueryHandler.
/// </summary>
public sealed class AbsenceQueryHandlerTests
{
    private readonly Mock<IAbsenceService> _serviceMock;
    private readonly AbsenceQueryHandler _sut;

    public AbsenceQueryHandlerTests()
    {
        _serviceMock = new Mock<IAbsenceService>();
        _sut = new AbsenceQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneAbsence()
    {
        var absence = new Absence { Id = 1, EleveId = 10, EstJustifiee = false };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        var result = await _sut.Handle(new GetAbsenceByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Absence?)null);

        var result = await _sut.Handle(new GetAbsenceByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var absences = new List<Absence>
        {
            new() { Id = 1, EleveId = 10 },
            new() { Id = 2, EleveId = 20 },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(absences);

        var result = await _sut.Handle(new GetAllAbsencesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Absence>(
            new List<Absence> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedAbsencesQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
