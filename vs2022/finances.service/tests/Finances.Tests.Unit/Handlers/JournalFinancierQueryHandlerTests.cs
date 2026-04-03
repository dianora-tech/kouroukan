using Finances.Application.Handlers;
using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using GnDapper.Models;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour JournalFinancierQueryHandler.
/// </summary>
public sealed class JournalFinancierQueryHandlerTests
{
    private readonly Mock<IJournalFinancierService> _serviceMock;
    private readonly JournalFinancierQueryHandler _sut;

    public JournalFinancierQueryHandlerTests()
    {
        _serviceMock = new Mock<IJournalFinancierService>();
        _sut = new JournalFinancierQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<JournalFinancier>(
            new List<JournalFinancier> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedJournalFinancierQuery(1, 20, null, null, null, null, null, null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
