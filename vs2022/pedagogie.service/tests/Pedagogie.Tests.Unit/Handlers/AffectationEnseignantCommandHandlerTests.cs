using FluentAssertions;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AffectationEnseignantCommandHandler.
/// </summary>
public sealed class AffectationEnseignantCommandHandlerTests
{
    private readonly Mock<IAffectationEnseignantService> _serviceMock;
    private readonly AffectationEnseignantCommandHandler _sut;

    public AffectationEnseignantCommandHandlerTests()
    {
        _serviceMock = new Mock<IAffectationEnseignantService>();
        _sut = new AffectationEnseignantCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneAffectation()
    {
        var command = new CreateAffectationEnseignantCommand(
            LiaisonId: 10,
            ClasseId: 5,
            MatiereId: 3,
            AnneeScolaireId: 1);

        var expected = new AffectationEnseignant
        {
            Id = 42,
            LiaisonId = 10,
            ClasseId = 5,
            MatiereId = 3,
            AnneeScolaireId = 1
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<AffectationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<AffectationEnseignant>(a =>
                a.LiaisonId == 10 &&
                a.ClasseId == 5 &&
                a.MatiereId == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateAffectationEnseignantCommand(
            Id: 1, LiaisonId: 10, ClasseId: 5,
            MatiereId: 3, AnneeScolaireId: 1, EstActive: true);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<AffectationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<AffectationEnseignant>(a => a.Id == 1 && a.EstActive),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateAffectationEnseignantCommand(
            Id: 999, LiaisonId: 10, ClasseId: 5,
            MatiereId: 3, AnneeScolaireId: 1, EstActive: false);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<AffectationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteAffectationEnseignantCommand(1);

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
        var command = new DeleteAffectationEnseignantCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
