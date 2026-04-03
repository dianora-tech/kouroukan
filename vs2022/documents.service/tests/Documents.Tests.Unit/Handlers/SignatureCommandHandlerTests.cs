using FluentAssertions;
using Documents.Application.Commands;
using Documents.Application.Handlers;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Documents.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SignatureCommandHandler.
/// </summary>
public sealed class SignatureCommandHandlerTests
{
    private readonly Mock<ISignatureService> _serviceMock;
    private readonly SignatureCommandHandler _sut;

    public SignatureCommandHandlerTests()
    {
        _serviceMock = new Mock<ISignatureService>();
        _sut = new SignatureCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneSignature()
    {
        var command = new CreateSignatureCommand(
            TypeId: 1,
            DocumentGenereId: 10,
            SignataireId: 5,
            OrdreSignature: 1,
            DateSignature: null,
            StatutSignature: "EnAttente",
            NiveauSignature: "Basique",
            MotifRefus: null,
            EstValidee: false,
            UserId: 1);

        var expected = new Signature
        {
            Id = 42,
            TypeId = 1,
            DocumentGenereId = 10,
            SignataireId = 5,
            StatutSignature = "EnAttente"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Signature>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Signature>(e =>
                e.TypeId == 1 &&
                e.DocumentGenereId == 10 &&
                e.SignataireId == 5 &&
                e.OrdreSignature == 1 &&
                e.StatutSignature == "EnAttente" &&
                e.NiveauSignature == "Basique"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateSignatureCommand(
            Id: 1,
            TypeId: 1,
            DocumentGenereId: 10,
            SignataireId: 5,
            OrdreSignature: 1,
            DateSignature: new DateTime(2025, 6, 20),
            StatutSignature: "Signe",
            NiveauSignature: "Avancee",
            MotifRefus: null,
            EstValidee: true,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Signature>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Signature>(e => e.Id == 1 && e.EstValidee && e.StatutSignature == "Signe"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateSignatureCommand(
            Id: 999, TypeId: 1, DocumentGenereId: 10, SignataireId: 5,
            OrdreSignature: 1, DateSignature: null, StatutSignature: "EnAttente",
            NiveauSignature: "Basique", MotifRefus: null, EstValidee: false, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Signature>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteSignatureCommand(1);

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
        var command = new DeleteSignatureCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
