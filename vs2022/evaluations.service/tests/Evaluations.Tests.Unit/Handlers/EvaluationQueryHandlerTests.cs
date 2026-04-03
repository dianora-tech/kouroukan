using FluentAssertions;
using GnDapper.Models;
using Evaluations.Application.Handlers;
using Evaluations.Application.Queries;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Evaluations.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour EvaluationQueryHandler.
/// </summary>
public sealed class EvaluationQueryHandlerTests
{
    private readonly Mock<IEvaluationService> _serviceMock;
    private readonly EvaluationQueryHandler _sut;

    public EvaluationQueryHandlerTests()
    {
        _serviceMock = new Mock<IEvaluationService>();
        _sut = new EvaluationQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetById_RetourneEvaluation()
    {
        var evaluation = new Evaluation { Id = 1, TypeId = 1, MatiereId = 10 };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);

        var result = await _sut.Handle(new GetEvaluationByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evaluation?)null);

        var result = await _sut.Handle(new GetEvaluationByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var evaluations = new List<Evaluation>
        {
            new() { Id = 1, TypeId = 1 },
            new() { Id = 2, TypeId = 2 },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluations);

        var result = await _sut.Handle(new GetAllEvaluationsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Evaluation>(
            new List<Evaluation> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedEvaluationsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPaged_AvecTypeId_RetourneResultatFiltre()
    {
        var paged = new PagedResult<Evaluation>(
            new List<Evaluation> { new() { Id = 1, TypeId = 2 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, 2, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedEvaluationsQuery(1, 20, null, null, 2),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}
