using FluentAssertions;
using Support.Application.Commands;
using Support.Application.Handlers;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Support.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour ConversationIAHandler.
/// </summary>
public sealed class ConversationIAHandlerTests
{
    private readonly Mock<IAideIAService> _serviceMock;
    private readonly ConversationIAHandler _sut;

    public ConversationIAHandlerTests()
    {
        _serviceMock = new Mock<IAideIAService>();
        _sut = new ConversationIAHandler(_serviceMock.Object);
    }

    // ─── CreerConversation ───

    [Fact]
    public async Task Handle_CreerConversation_AppelleService_EtRetourneConversation()
    {
        var command = new CreerConversationIACommand(UtilisateurId: 10, UserId: 1);

        var expected = new ConversationIA
        {
            Id = 42,
            UtilisateurId = 10,
            EstActive = true
        };

        _serviceMock
            .Setup(s => s.CreerConversationAsync(10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        result.UtilisateurId.Should().Be(10);
        _serviceMock.Verify(s => s.CreerConversationAsync(10, 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── EnvoyerMessage ───

    [Fact]
    public async Task Handle_EnvoyerMessage_AppelleService_EtRetourneReponse()
    {
        var command = new EnvoyerMessageIACommand(
            ConversationId: 1,
            Question: "Comment reinitialiser mon mot de passe?",
            UserId: 1);

        _serviceMock
            .Setup(s => s.GenererReponseAsync(1, "Comment reinitialiser mon mot de passe?", It.IsAny<CancellationToken>()))
            .ReturnsAsync("Pour reinitialiser votre mot de passe, allez dans Parametres...");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("reinitialiser");
        _serviceMock.Verify(s => s.GenererReponseAsync(1, "Comment reinitialiser mon mot de passe?", It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── GenererReponseTicket ───

    [Fact]
    public async Task Handle_GenererReponseTicket_AppelleService_EtRetourneReponse()
    {
        var command = new GenererReponseIATicketCommand(TicketId: 5);

        _serviceMock
            .Setup(s => s.SuggererReponseTicketAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync("Suggestion de reponse IA pour le ticket");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNullOrEmpty();
        _serviceMock.Verify(s => s.SuggererReponseTicketAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── GetMessages ───

    [Fact]
    public async Task Handle_GetMessages_RetourneListe()
    {
        var messages = new List<MessageIA>
        {
            new() { Id = 1, Role = "user", Contenu = "Question" },
            new() { Id = 2, Role = "assistant", Contenu = "Reponse" },
        };
        _serviceMock
            .Setup(s => s.GetMessagesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        var result = await _sut.Handle(new GetMessagesConversationIAQuery(1), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetMessages_RetourneListeVide_SiAucunMessage()
    {
        _serviceMock
            .Setup(s => s.GetMessagesAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MessageIA>());

        var result = await _sut.Handle(new GetMessagesConversationIAQuery(999), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
