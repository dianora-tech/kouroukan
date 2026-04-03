using FluentAssertions;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour CompetenceEnseignantCommandHandler.
/// </summary>
public sealed class CompetenceEnseignantCommandHandlerTests
{
    private readonly Mock<ICompetenceEnseignantService> _serviceMock;
    private readonly CompetenceEnseignantCommandHandler _sut;

    public CompetenceEnseignantCommandHandlerTests()
    {
        _serviceMock = new Mock<ICompetenceEnseignantService>();
        _sut = new CompetenceEnseignantCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneCompetence()
    {
        var command = new CreateCompetenceEnseignantCommand(
            UserId: 10,
            MatiereId: 3,
            CycleEtude: "College");

        var expected = new CompetenceEnseignant
        {
            Id = 42,
            UserId = 10,
            MatiereId = 3,
            CycleEtude = "College"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<CompetenceEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<CompetenceEnseignant>(c =>
                c.UserId == 10 &&
                c.MatiereId == 3 &&
                c.CycleEtude == "College"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteCompetenceEnseignantCommand(1);

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
        var command = new DeleteCompetenceEnseignantCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
