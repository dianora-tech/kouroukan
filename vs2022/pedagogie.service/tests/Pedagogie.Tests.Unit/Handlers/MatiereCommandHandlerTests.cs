using FluentAssertions;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour MatiereCommandHandler.
/// </summary>
public sealed class MatiereCommandHandlerTests
{
    private readonly Mock<IMatiereService> _serviceMock;
    private readonly MatiereCommandHandler _sut;

    public MatiereCommandHandlerTests()
    {
        _serviceMock = new Mock<IMatiereService>();
        _sut = new MatiereCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneMatiere()
    {
        var command = new CreateMatiereCommand(
            Name: "Mathematiques",
            Description: "Cours de maths",
            TypeId: 1,
            Code: "MATH");

        var expected = new Matiere
        {
            Id = 42,
            Name = "Mathematiques",
            TypeId = 1,
            Code = "MATH"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Matiere>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Matiere>(m =>
                m.Name == "Mathematiques" &&
                m.TypeId == 1 &&
                m.Code == "MATH"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateMatiereCommand(
            Id: 1,
            Name: "Mathematiques",
            Description: "Mise a jour",
            TypeId: 1,
            Code: "MATH");

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Matiere>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Matiere>(m => m.Id == 1 && m.Name == "Mathematiques"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateMatiereCommand(
            Id: 999, Name: "X", Description: null, TypeId: 1, Code: "X");

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Matiere>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteMatiereCommand(1);

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
        var command = new DeleteMatiereCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
