using GnDapper.Models;

namespace GnDapper.Test.Models;

public sealed class PagedResultTests
{
    [Fact]
    public void TotalPages_ItemsNotExactlyDivisible_RoundsUp()
    {
        // Arrange & Act
        var result = new PagedResult<string>(
            Items: ["a", "b", "c"],
            TotalCount: 25,
            Page: 1,
            PageSize: 10);

        // Assert
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_ItemsExactlyDivisible_ReturnsExactCount()
    {
        // Arrange & Act
        var result = new PagedResult<string>(
            Items: ["a", "b"],
            TotalCount: 20,
            Page: 1,
            PageSize: 10);

        // Assert
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public void HasPrevious_FirstPage_ReturnsFalse()
    {
        // Arrange & Act
        var result = new PagedResult<string>(
            Items: ["a"],
            TotalCount: 30,
            Page: 1,
            PageSize: 10);

        // Assert
        result.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void HasPrevious_SecondPage_ReturnsTrue()
    {
        // Arrange & Act
        var result = new PagedResult<string>(
            Items: ["a"],
            TotalCount: 30,
            Page: 2,
            PageSize: 10);

        // Assert
        result.HasPrevious.Should().BeTrue();
    }

    [Fact]
    public void HasNext_LastPage_ReturnsFalse()
    {
        // Arrange & Act
        var result = new PagedResult<string>(
            Items: ["a"],
            TotalCount: 30,
            Page: 3,
            PageSize: 10);

        // Assert
        result.HasNext.Should().BeFalse();
    }

    [Fact]
    public void HasNext_FirstPageOfMultiple_ReturnsTrue()
    {
        // Arrange & Act
        var result = new PagedResult<string>(
            Items: ["a"],
            TotalCount: 30,
            Page: 1,
            PageSize: 10);

        // Assert
        result.HasNext.Should().BeTrue();
    }

    [Fact]
    public void EmptyResult_ZeroItems_HasZeroTotalPages()
    {
        // Arrange & Act
        var result = new PagedResult<string>(
            Items: [],
            TotalCount: 0,
            Page: 1,
            PageSize: 10);

        // Assert
        result.TotalPages.Should().Be(0);
        result.HasPrevious.Should().BeFalse();
        result.HasNext.Should().BeFalse();
    }

    [Fact]
    public void TotalPages_PageSizeZero_ReturnsZero()
    {
        // Arrange & Act
        var result = new PagedResult<string>(
            Items: [],
            TotalCount: 25,
            Page: 1,
            PageSize: 0);

        // Assert
        result.TotalPages.Should().Be(0);
    }
}
