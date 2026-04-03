using FluentAssertions;
using Communication.Application.Commands;
using Communication.Application.Handlers;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Communication.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour MessageCommandHandler.
/// </summary>
public sealed class MessageCommandHandlerTests
{
    private readonly Mock<IMessageService> _serviceMock;
    private readonly MessageCommandHandler _sut;

    public MessageCommandHandlerTests()
    {
        _serviceMock = new Mock<IMessageService>();
        _sut = new MessageCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneMessage()
    {
        var command = new CreateMessageCommand(
            TypeId: 1,
            ExpediteurId: 10,
            DestinataireId: 5,
            Sujet: "Sujet test",
            Contenu: "Contenu test",
            EstLu: false,
            DateLecture: null,
            GroupeDestinataire: null,
            UserId: 1);

        var expected = new Message
        {
            Id = 42,
            TypeId = 1,
            ExpediteurId = 10,
            Sujet = "Sujet test"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Message>(m =>
                m.TypeId == 1 &&
                m.ExpediteurId == 10 &&
                m.DestinataireId == 5 &&
                m.Sujet == "Sujet test" &&
                m.Contenu == "Contenu test"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateMessageCommand(
            Id: 1,
            TypeId: 1,
            ExpediteurId: 10,
            DestinataireId: 5,
            Sujet: "Sujet modifie",
            Contenu: "Contenu modifie",
            EstLu: true,
            DateLecture: DateTime.Now,
            GroupeDestinataire: null,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Message>(m => m.Id == 1 && m.EstLu),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateMessageCommand(
            Id: 999, TypeId: 1, ExpediteurId: 10, DestinataireId: null,
            Sujet: "Test", Contenu: "Test", EstLu: false, DateLecture: null,
            GroupeDestinataire: "Tous", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteMessageCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistant()
    {
        var command = new DeleteMessageCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
