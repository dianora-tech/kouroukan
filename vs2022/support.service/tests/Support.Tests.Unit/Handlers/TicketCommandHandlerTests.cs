using FluentAssertions;
using Support.Application.Commands;
using Support.Application.Handlers;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Support.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour TicketCommandHandler.
/// </summary>
public sealed class TicketCommandHandlerTests
{
    private readonly Mock<ITicketService> _serviceMock;
    private readonly TicketCommandHandler _sut;

    public TicketCommandHandlerTests()
    {
        _serviceMock = new Mock<ITicketService>();
        _sut = new TicketCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneTicket()
    {
        var command = new CreateTicketCommand(
            Name: "Ticket test",
            Description: "Description",
            TypeId: 1,
            AuteurId: 10,
            Sujet: "Probleme de connexion",
            Contenu: "Je ne peux pas me connecter",
            Priorite: "Haute",
            CategorieTicket: "Bug",
            ModuleConcerne: "Auth",
            PieceJointeUrl: "https://example.com/file.pdf",
            UserId: 1);

        var expected = new Ticket
        {
            Id = 42,
            Name = "Ticket test",
            Sujet = "Probleme de connexion",
            Contenu = "Je ne peux pas me connecter",
            Priorite = "Haute",
            CategorieTicket = "Bug",
            AuteurId = 10
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Ticket>(t =>
                t.Name == "Ticket test" &&
                t.Sujet == "Probleme de connexion" &&
                t.Contenu == "Je ne peux pas me connecter" &&
                t.Priorite == "Haute" &&
                t.CategorieTicket == "Bug" &&
                t.AuteurId == 10 &&
                t.ModuleConcerne == "Auth" &&
                t.PieceJointeUrl == "https://example.com/file.pdf" &&
                t.UserId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateTicketCommand(
            Id: 1,
            Name: "Ticket modifie",
            Description: "Desc",
            TypeId: 1,
            AuteurId: 10,
            Sujet: "Sujet modifie",
            Contenu: "Contenu modifie",
            Priorite: "Critique",
            StatutTicket: "EnCours",
            CategorieTicket: "Question",
            ModuleConcerne: "Dashboard",
            AssigneAId: 5,
            DateResolution: new DateTime(2025, 6, 15),
            NoteSatisfaction: 4,
            PieceJointeUrl: null,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Ticket>(t =>
                t.Id == 1 &&
                t.Name == "Ticket modifie" &&
                t.StatutTicket == "EnCours" &&
                t.AssigneAId == 5 &&
                t.NoteSatisfaction == 4),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateTicketCommand(
            Id: 999, Name: "Test", Description: null, TypeId: 1, AuteurId: 1,
            Sujet: "Test", Contenu: "Test", Priorite: "Basse", StatutTicket: "Ouvert",
            CategorieTicket: "Bug", ModuleConcerne: null, AssigneAId: null,
            DateResolution: null, NoteSatisfaction: null, PieceJointeUrl: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteTicketCommand(1);

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
        var command = new DeleteTicketCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── AddReponse ───

    [Fact]
    public async Task Handle_AddReponseCommand_AppelleService_EtRetourneReponse()
    {
        var command = new AddReponseTicketCommand(
            TicketId: 1,
            AuteurId: 10,
            Contenu: "Voici la reponse",
            EstReponseIA: false,
            EstInterne: true,
            UserId: 1);

        var expected = new ReponseTicket
        {
            Id = 7,
            TicketId = 1,
            AuteurId = 10,
            Contenu = "Voici la reponse",
            EstReponseIA = false,
            EstInterne = true
        };

        _serviceMock
            .Setup(s => s.AddReponseAsync(It.IsAny<ReponseTicket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(7);
        _serviceMock.Verify(s => s.AddReponseAsync(
            It.Is<ReponseTicket>(r =>
                r.TicketId == 1 &&
                r.AuteurId == 10 &&
                r.Contenu == "Voici la reponse" &&
                r.EstReponseIA == false &&
                r.EstInterne == true &&
                r.UserId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
