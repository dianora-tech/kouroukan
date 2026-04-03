using FluentAssertions;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Handlers;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour TransfertCommandHandler.
/// </summary>
public sealed class TransfertCommandHandlerTests
{
    private readonly Mock<ITransfertService> _serviceMock;
    private readonly TransfertCommandHandler _sut;

    public TransfertCommandHandlerTests()
    {
        _serviceMock = new Mock<ITransfertService>();
        _sut = new TransfertCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneTransfert()
    {
        var command = new CreateTransfertCommand(
            EleveId: 10,
            CompanyOrigineId: 1,
            CompanyCibleId: 2,
            Motif: "Demenagement",
            Documents: null);

        var expected = new Transfert
        {
            Id = 42,
            EleveId = 10,
            CompanyOrigineId = 1,
            CompanyCibleId = 2,
            Statut = "EnAttente"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Transfert>(t =>
                t.EleveId == 10 &&
                t.CompanyOrigineId == 1 &&
                t.CompanyCibleId == 2 &&
                t.Motif == "Demenagement"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_MappeDocuments()
    {
        var command = new CreateTransfertCommand(
            EleveId: 10,
            CompanyOrigineId: 1,
            CompanyCibleId: 2,
            Motif: "Rapprochement familial",
            Documents: "{\"certificat\":\"scan.pdf\"}");

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Transfert { Id = 1 });

        await _sut.Handle(command, CancellationToken.None);

        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Transfert>(t => t.Documents == "{\"certificat\":\"scan.pdf\"}"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Accept ───

    [Fact]
    public async Task Handle_AcceptCommand_AppelleService_EtRetourneTrue()
    {
        var command = new AcceptTransfertCommand(1);

        _serviceMock
            .Setup(s => s.AcceptAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.AcceptAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcceptCommand_RetourneFalse_SiInexistant()
    {
        var command = new AcceptTransfertCommand(999);

        _serviceMock
            .Setup(s => s.AcceptAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Reject ───

    [Fact]
    public async Task Handle_RejectCommand_AppelleService_EtRetourneTrue()
    {
        var command = new RejectTransfertCommand(1);

        _serviceMock
            .Setup(s => s.RejectAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.RejectAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RejectCommand_RetourneFalse_SiInexistant()
    {
        var command = new RejectTransfertCommand(999);

        _serviceMock
            .Setup(s => s.RejectAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Complete ───

    [Fact]
    public async Task Handle_CompleteCommand_AppelleService_EtRetourneTrue()
    {
        var command = new CompleteTransfertCommand(1);

        _serviceMock
            .Setup(s => s.CompleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.CompleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CompleteCommand_RetourneFalse_SiInexistant()
    {
        var command = new CompleteTransfertCommand(999);

        _serviceMock
            .Setup(s => s.CompleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
