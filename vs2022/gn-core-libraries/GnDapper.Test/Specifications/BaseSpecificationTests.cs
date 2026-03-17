using GnDapper.Specifications;
using GnDapper.Test.Helpers;

namespace GnDapper.Test.Specifications;

public sealed class BaseSpecificationTests
{
    // ========================================================================
    // Specification vide
    // ========================================================================

    [Fact]
    public void EmptySpecification_AllPropertiesAreNull()
    {
        // Arrange & Act
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Assert
        spec.WhereClause.Should().BeNull();
        spec.Parameters.Should().BeNull();
        spec.OrderByClause.Should().BeNull();
        spec.Skip.Should().BeNull();
        spec.Take.Should().BeNull();
    }

    // ========================================================================
    // Where
    // ========================================================================

    [Fact]
    public void Where_SingleClause_SetsWhereClause()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.Where("name = @Name", new { Name = "test" });

        // Assert
        spec.WhereClause.Should().Be("name = @Name");
    }

    [Fact]
    public void Where_MultipleClauses_CombinesWithAnd()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.Where("name = @Name", new { Name = "test" })
            .Where("id > @MinId", new { MinId = 5 });

        // Assert
        spec.WhereClause.Should().Be("name = @Name AND id > @MinId");
    }

    [Fact]
    public void Where_WithParameters_StoresParameterValues()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.Where("name = @Name", new { Name = "test" });

        // Assert
        spec.Parameters.Should().NotBeNull();
        var dict = (IDictionary<string, object?>)spec.Parameters!;
        dict["Name"].Should().Be("test");
    }

    [Fact]
    public void Where_MultipleCallsWithParameters_MergesAllParameters()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.Where("name = @Name", new { Name = "test" })
            .Where("id > @MinId", new { MinId = 5 });

        // Assert
        var dict = (IDictionary<string, object?>)spec.Parameters!;
        dict.Should().ContainKey("Name");
        dict.Should().ContainKey("MinId");
        dict["Name"].Should().Be("test");
        dict["MinId"].Should().Be(5);
    }

    [Fact]
    public void Where_WithoutParameters_SetsWhereClauseOnly()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.Where("id > 0");

        // Assert
        spec.WhereClause.Should().Be("id > 0");
        spec.Parameters.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Where_NullOrEmptyClause_ThrowsArgumentException(string? clause)
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        var act = () => spec.Where(clause!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // ========================================================================
    // OrderBy
    // ========================================================================

    [Fact]
    public void OrderByAsc_SingleColumn_SetsOrderByClauseWithAsc()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.OrderByAsc("name");

        // Assert
        spec.OrderByClause.Should().Be("name ASC");
    }

    [Fact]
    public void OrderByDesc_SingleColumn_SetsOrderByClauseWithDesc()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.OrderByDesc("created_at");

        // Assert
        spec.OrderByClause.Should().Be("created_at DESC");
    }

    [Fact]
    public void OrderBy_MultipleCalls_CombinesWithComma()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.OrderByAsc("last_name")
            .OrderByDesc("created_at");

        // Assert
        spec.OrderByClause.Should().Be("last_name ASC, created_at DESC");
    }

    // ========================================================================
    // Pagination
    // ========================================================================

    [Fact]
    public void WithPaging_FirstPage_SetsSkipToZeroAndTakeToPageSize()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.WithPaging(1, 20);

        // Assert
        spec.Skip.Should().Be(0);
        spec.Take.Should().Be(20);
    }

    [Fact]
    public void WithPaging_ThirdPage_SetsCorrectSkipAndTake()
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        spec.WithPaging(3, 10);

        // Assert
        spec.Skip.Should().Be(20);
        spec.Take.Should().Be(10);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void WithPaging_InvalidPageOrPageSize_ThrowsArgumentOutOfRangeException(int page, int pageSize)
    {
        // Arrange
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        var act = () => spec.WithPaging(page, pageSize);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    // ========================================================================
    // Chainage fluide
    // ========================================================================

    [Fact]
    public void FluentChaining_AllClauses_WorksTogether()
    {
        // Arrange & Act
        var spec = new BaseSpecification<SimpleTestEntity>()
            .Where("name ILIKE @Name", new { Name = "%test%" })
            .Where("id > @MinId", new { MinId = 0 })
            .OrderByAsc("name")
            .OrderByDesc("id")
            .WithPaging(2, 15);

        // Assert
        spec.WhereClause.Should().Be("name ILIKE @Name AND id > @MinId");
        spec.OrderByClause.Should().Be("name ASC, id DESC");
        spec.Skip.Should().Be(15);
        spec.Take.Should().Be(15);

        var dict = (IDictionary<string, object?>)spec.Parameters!;
        dict["Name"].Should().Be("%test%");
        dict["MinId"].Should().Be(0);
    }
}
