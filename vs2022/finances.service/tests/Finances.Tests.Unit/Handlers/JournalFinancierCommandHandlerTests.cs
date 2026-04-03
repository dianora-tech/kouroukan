using Finances.Application.Commands;
using Finances.Application.Handlers;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour JournalFinancierCommandHandler.
/// </summary>
public sealed class JournalFinancierCommandHandlerTests
{
    private readonly Mock<IJournalFinancierService> _serviceMock;
    private readonly JournalFinancierCommandHandler _sut;

    public JournalFinancierCommandHandlerTests()
    {
        _serviceMock = new Mock<IJournalFinancierService>();
        _sut = new JournalFinancierCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneEntree()
    {
        var command = new CreateJournalFinancierCommand(
            CompanyId: 1, Type: "Recette", Categorie: "Scolarite",
            Montant: 150000m, Description: "Paiement scolarite",
            ReferenceExterne: null, ModePaiement: "OrangeMoney",
            DateOperation: new DateTime(2025, 9, 1), EleveId: 10, ParentUserId: 5);

        var expected = new JournalFinancier { Id = 42, Type = "Recette", Montant = 150000m };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<JournalFinancier>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<JournalFinancier>(e => e.CompanyId == 1 && e.Type == "Recette"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_UtiliseDateOperationDuCommand_SiFournie()
    {
        var dateOperation = new DateTime(2025, 10, 15);
        var command = new CreateJournalFinancierCommand(
            CompanyId: 1, Type: "Depense", Categorie: "Personnel",
            Montant: 500000m, Description: "Salaire",
            ReferenceExterne: null, ModePaiement: "Especes",
            DateOperation: dateOperation, EleveId: null, ParentUserId: null);

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<JournalFinancier>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JournalFinancier { Id = 1 });

        await _sut.Handle(command, CancellationToken.None);

        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<JournalFinancier>(e => e.DateOperation == dateOperation),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
