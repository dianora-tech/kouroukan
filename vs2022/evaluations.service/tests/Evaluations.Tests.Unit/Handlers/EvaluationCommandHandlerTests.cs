using FluentAssertions;
using Evaluations.Application.Commands;
using Evaluations.Application.Handlers;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Evaluations.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour EvaluationCommandHandler.
/// </summary>
public sealed class EvaluationCommandHandlerTests
{
    private readonly Mock<IEvaluationService> _serviceMock;
    private readonly EvaluationCommandHandler _sut;

    public EvaluationCommandHandlerTests()
    {
        _serviceMock = new Mock<IEvaluationService>();
        _sut = new EvaluationCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneEvaluation()
    {
        var command = new CreateEvaluationCommand(
            TypeId: 1,
            MatiereId: 10,
            ClasseId: 5,
            EnseignantId: 3,
            DateEvaluation: new DateTime(2025, 10, 15),
            Coefficient: 2m,
            NoteMaximale: 20m,
            Trimestre: 1,
            AnneeScolaireId: 1,
            UserId: 1);

        var expected = new Evaluation
        {
            Id = 42,
            TypeId = 1,
            MatiereId = 10,
            ClasseId = 5,
            EnseignantId = 3,
            Coefficient = 2m,
            NoteMaximale = 20m,
            Trimestre = 1
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Evaluation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Evaluation>(e =>
                e.TypeId == 1 &&
                e.MatiereId == 10 &&
                e.ClasseId == 5 &&
                e.EnseignantId == 3 &&
                e.Coefficient == 2m &&
                e.NoteMaximale == 20m &&
                e.Trimestre == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateEvaluationCommand(
            Id: 1,
            TypeId: 1,
            MatiereId: 10,
            ClasseId: 5,
            EnseignantId: 3,
            DateEvaluation: new DateTime(2025, 10, 15),
            Coefficient: 3m,
            NoteMaximale: 20m,
            Trimestre: 2,
            AnneeScolaireId: 1,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Evaluation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Evaluation>(e => e.Id == 1 && e.Trimestre == 2 && e.Coefficient == 3m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateEvaluationCommand(
            Id: 999, TypeId: 1, MatiereId: 10, ClasseId: 5, EnseignantId: 3,
            DateEvaluation: DateTime.Today, Coefficient: 1m, NoteMaximale: 20m,
            Trimestre: 1, AnneeScolaireId: 1, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Evaluation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteEvaluationCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistante()
    {
        var command = new DeleteEvaluationCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
