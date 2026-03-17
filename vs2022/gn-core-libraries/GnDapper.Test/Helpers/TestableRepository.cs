using GnDapper.Connection;
using GnDapper.Entities;
using GnDapper.Options;
using GnDapper.Repositories;
using GnDapper.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GnDapper.Test.Helpers;

public class TestableRepository<T> : Repository<T> where T : class, IEntity
{
    public TestableRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<T>> logger,
        IOptions<GnDapperOptions> options)
        : base(connectionFactory, logger, options)
    {
    }

    public new string BuildSelectSql(ISpecification<T> spec) => base.BuildSelectSql(spec);

    public new string BuildCountSql(ISpecification<T> spec) => base.BuildCountSql(spec);

    public string GetTableName() => Metadata.TableName;

    public string[] GetInsertColumnNames() =>
        Metadata.InsertColumns.Select(c => c.ColumnName).ToArray();

    public string[] GetUpdateColumnNames() =>
        Metadata.UpdateColumns.Select(c => c.ColumnName).ToArray();

    public string[] GetInsertPropertyNames() =>
        Metadata.InsertColumns.Select(c => c.PropertyName).ToArray();

    public string[] GetUpdatePropertyNames() =>
        Metadata.UpdateColumns.Select(c => c.PropertyName).ToArray();
}
