using System.Text;
using SequelNet;
using SequelNet.Connector;

namespace Tests;

public class AdvancedQueryRenderingTests
{
    [Test]
    public void Join_RendersAliasesQualifiedColumnsAndMultiplePredicates()
    {
        var orders = Schema("orders", "id", "customer_id", "tenant_id");
        var customers = Schema("customers", "id", "tenant_id", "name");
        var joinConditions = new JoinColumnPair("o", "customer_id", "id")
            .AND("o", "tenant_id", "tenant_id");
        var query = new Query(orders, "o")
            .Select("o", "id", null)
            .Select("c", "name", null)
            .InnerJoin(customers, "c", joinConditions)
            .Where("o", "tenant_id", WhereComparison.EqualsTo, 7);

        Assert.That(query.BuildCommand(new AdvancedConnector()), Is.EqualTo(
            " SELECT `o`.`id`,`c`.`name` FROM `orders` `o` INNER JOIN `customers` `c` ON `o`.`customer_id` = `c`.`id` AND `o`.`tenant_id` = `c`.`tenant_id` WHERE `o`.`tenant_id` = 7"));
    }

    [Test]
    public void Join_RendersRawRightTableExpressions()
    {
        var query = new Query(Schema("orders", "id"), "o")
            .SelectAllTableColumns()
            .LeftJoin("(SELECT 1 AS order_id)", "derived", new JoinColumnPair("o", "id", "order_id"));

        Assert.That(query.BuildCommand(new AdvancedConnector()), Is.EqualTo(
            " SELECT `o`.* FROM `orders` `o` LEFT JOIN (SELECT 1 AS order_id) `derived` ON `o`.`id` = `derived`.`order_id`"));
    }

    [Test]
    public void QueryCombinations_RenderInDeclarationOrderAndCanBeCleared()
    {
        var query = new Query("orders").Select("id")
            .Union(new Query("archived_orders").Select("id"), all: true)
            .Intersect(new Query("active_orders").Select("id"))
            .Except(new Query("blocked_orders").Select("id"), all: true);
        var connector = new AdvancedConnector();
        var combinedSql = query.BuildCommand(connector);

        Assert.That(combinedSql, Does.Contain("UNION ALL"));
        Assert.That(combinedSql, Does.Contain("INTERSECT"));
        Assert.That(combinedSql, Does.Contain("EXCEPT ALL"));

        query.ClearCombinations();
        Assert.That(query.BuildCommand(connector), Is.EqualTo(" SELECT `id` FROM `orders`"));
    }

    [Test]
    public void Insert_RendersConfiguredConflictHandlers()
    {
        var schema = Schema("orders", "id", "status");
        var doNothing = new Query(schema).Insert("id", 1).SetIgnoreErrors(true);
        var doUpdate = new Query(schema)
            .Insert("id", 1)
            .SetOnConflictDoUpdate(new OnConflict().Update("status", "new"));
        var connector = new AdvancedConnector();

        Assert.That(doNothing.BuildCommand(connector), Is.EqualTo(
            "INSERT INTO `orders` (`id`) VALUES (1) ON CONFLICT DO NOTHING"));
        Assert.That(doUpdate.BuildCommand(connector), Is.EqualTo(
            "INSERT INTO `orders` (`id`) VALUES (1) ON CONFLICT DO UPDATE SET `status`='new'"));
    }

    [Test]
    public void CreateAllTableElements_RendersIndexAndForeignKeyOperations()
    {
        var schema = Schema("orders", "id", "customer_id");
        schema.AddIndex("IX_orders_customer", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, "customer_id");
        schema.AddForeignKey("FK_orders_customer", "customer_id", "customers", "id", TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.None);

        var sql = new Query(schema).CreateAllTableElements().BuildCommand(new AdvancedConnector());

        Assert.That(sql, Does.Contain("ALTER TABLE `orders`"));
        Assert.That(sql, Does.Contain("INDEX `IX_orders_customer` (`customer_id`)"));
        Assert.That(sql, Does.Contain("`FK_orders_customer` FOREIGN KEY (`customer_id`) REFERENCES customers (`id`) ON DELETE CASCADE"));
    }

    private static TableSchema Schema(string name, params string[] columns)
    {
        var schema = new TableSchema(name, null);
        foreach (var column in columns)
            schema.AddColumn(new TableSchema.Column { Name = column, Type = typeof(int) });
        return schema;
    }

    private sealed class AdvancedConnector : ConnectorBase
    {
        private static readonly LanguageFactory AdvancedLanguage = new AdvancedLanguageFactory();

        public override LanguageFactory Language => AdvancedLanguage;

        public override IConnectorFactory Factory => throw new NotSupportedException();

        public override int ExecuteScript(string querySql) => throw new NotSupportedException();

        public override Task<int> ExecuteScriptAsync(string querySql, CancellationToken? cancellationToken = null) => throw new NotSupportedException();

        public override object GetLastInsertID() => throw new NotSupportedException();

        public override Task<object> GetLastInsertIdAsync() => throw new NotSupportedException();

        public override bool CheckIfTableExists(string tableName) => throw new NotSupportedException();

        public override Task<bool> CheckIfTableExistsAsync(string tableName) => throw new NotSupportedException();
    }

    private sealed class AdvancedLanguageFactory : LanguageFactory
    {
        public override void BuildLimitOffset(Query query, bool top, StringBuilder outputBuilder)
        {
        }

        public override bool InsertSupportsOnConflictDoNothing => true;

        public override bool InsertSupportsOnConflictDoUpdate => true;

        public override void BuildOnConflictDoNothing(StringBuilder outputBuilder, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
        {
            outputBuilder.Append("ON CONFLICT DO NOTHING");
        }

        public override void BuildOnConflictDoUpdate(StringBuilder outputBuilder, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
        {
            outputBuilder.Append("ON CONFLICT DO UPDATE SET ");
            for (var index = 0; index < conflict.Updates.Count; index++)
            {
                if (index > 0)
                    outputBuilder.Append(',');
                var update = conflict.Updates[index];
                outputBuilder.Append(WrapFieldName(update.ColumnName));
                outputBuilder.Append('=');
                outputBuilder.Append(PrepareValue(conn, update.Second, relatedQuery));
            }
        }

        public override void BuildCreateIndex(TableSchema.Index index, StringBuilder outputBuilder, Query qry, ConnectorBase conn)
        {
            outputBuilder.Append("INDEX ");
            outputBuilder.Append(WrapFieldName(index.Name));
            outputBuilder.Append(" (");
            for (var columnIndex = 0; columnIndex < index.Columns.Length; columnIndex++)
            {
                if (columnIndex > 0)
                    outputBuilder.Append(',');
                outputBuilder.Append(WrapFieldName(index.Columns[columnIndex].Target.Value!.ToString()!));
            }
            outputBuilder.Append(')');
        }
    }
}
