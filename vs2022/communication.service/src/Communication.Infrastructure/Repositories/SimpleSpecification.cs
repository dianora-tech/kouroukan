using GnDapper.Entities;
using GnDapper.Specifications;

namespace Communication.Infrastructure.Repositories;

internal sealed class SimpleSpecification<T> : ISpecification<T> where T : IEntity
{
    public SimpleSpecification(string? whereClause, object? parameters, string? orderByClause, int? skip, int? take)
    {
        WhereClause = whereClause;
        Parameters = parameters;
        OrderByClause = orderByClause;
        Skip = skip;
        Take = take;
    }

    public string? WhereClause { get; }
    public object? Parameters { get; }
    public string? OrderByClause { get; }
    public int? Skip { get; }
    public int? Take { get; }
}
