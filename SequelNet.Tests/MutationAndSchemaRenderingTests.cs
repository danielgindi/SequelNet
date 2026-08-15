using System.Text;
using SequelNet;
using SequelNet.Connector;

namespace Tests;

public class MutationAndSchemaRenderingTests
{
    [Test]
    public void Insert_RendersColumnsValuesAndLiteralExpressions()
    {
        var query = new Query(CreateOrdersSchema())
            .Insert("id", 7)
            .Insert("status", "new")
            .Insert("created_at", "CURRENT_TIMESTAMP", ColumnNameIsLiteral: true);

        Assert.That(query.BuildCommand(new RenderingConnector()), Is.EqualTo(
            "INSERT INTO `orders` (`id`,`status`,`created_at`) VALUES (7,'new',CURRENT_TIMESTAMP)"));
    }

    [Test]
    public void Update_RendersValuesColumnReferencesFiltersAndPaging()
    {
        var query = new Query(CreateOrdersSchema())
            .Update("status", "complete")
            .UpdateFromOtherColumn("previous_status", "status")
            .Where("id", 7)
            .OrderBy("id", SortDirection.DESC);
        query.Limit = 1;

        Assert.That(query.BuildCommand(new RenderingConnector()), Is.EqualTo(
            "UPDATE `orders` SET `status`='complete',`previous_status`=`status` WHERE `id` = 7 ORDER BY `id` DESC LIMIT 1"));
    }

    [Test]
    public void Delete_RendersFiltersOrderingAndOffsetPaging()
    {
        var query = new Query(CreateOrdersSchema())
            .Delete()
            .Where("status", "expired")
            .OrderBy("id", SortDirection.ASC);
        query.Limit = 5;
        query.Offset = 10;

        Assert.That(query.BuildCommand(new RenderingConnector()), Is.EqualTo(
            "DELETE  FROM `orders` WHERE `status` = 'expired' ORDER BY `id` ASC LIMIT 5 OFFSET 10"));
    }

    [Test]
    public void Select_RendersGroupingHavingOrderingAndPaging()
    {
        var query = new Query(CreateOrdersSchema())
            .Select("status")
            .SelectLiteral("COUNT(*)", "count")
            .GroupBy("status", SortDirection.ASC)
            .Having("status", WhereComparison.NotEqualsTo, null)
            .OrderBy("status", SortDirection.DESC);
        query.Limit = 25;
        query.Offset = 50;

        Assert.That(query.BuildCommand(new RenderingConnector()), Is.EqualTo(
            " SELECT `status`,COUNT(*) AS `count` FROM `orders` GROUP BY `status` ASC HAVING `status` IS NOT NULL ORDER BY `status` DESC LIMIT 25 OFFSET 50"));
    }

    [Test]
    public void CreateTable_RendersColumnsPrimaryKeyDefaultsAndTableOptions()
    {
        var schema = CreateOrdersSchema();
        schema.SetTableOption("ENGINE", "InnoDB");
        var query = new Query(schema).CreateTable();

        var sql = query.BuildCommand(new RenderingConnector());

        Assert.Multiple(() =>
        {
            Assert.That(sql, Does.StartWith("CREATE TABLE `orders`("));
            Assert.That(sql, Does.Contain("`id` INTEGER NOT NULL"));
            Assert.That(sql, Does.Contain("`status` TEXT DEFAULT 'pending'"));
            Assert.That(sql, Does.Contain("CONSTRAINT `PK_orders` PRIMARY KEY(`id`)"));
            Assert.That(sql, Does.EndWith(" ENGINE=InnoDB"));
        });
    }

    [Test]
    public void DropColumn_RendersASeparatedAlterTableCommand()
    {
        var query = new Query(CreateOrdersSchema()).DropColumn("obsolete_column");

        Assert.That(query.BuildCommand(new RenderingConnector()), Is.EqualTo(
            "ALTER TABLE `orders` DROP COLUMN `obsolete_column`"));
    }

    private static TableSchema CreateOrdersSchema()
    {
        var schema = new TableSchema("orders", null);
        schema.AddColumn(new TableSchema.Column { Name = "id", Type = typeof(int), IsPrimaryKey = true, Nullable = false });
        schema.AddColumn(new TableSchema.Column { Name = "status", Type = typeof(string), Nullable = true, Default = "pending" });
        schema.AddColumn(new TableSchema.Column { Name = "previous_status", Type = typeof(string), Nullable = true });
        schema.AddColumn(new TableSchema.Column { Name = "created_at", Type = typeof(DateTime), Nullable = false });
        return schema;
    }

    private sealed class RenderingConnector : ConnectorBase
    {
        private static readonly LanguageFactory RenderingLanguage = new RenderingLanguageFactory();

        public override LanguageFactory Language => RenderingLanguage;

        public override IConnectorFactory Factory => throw new NotSupportedException();

        public override int ExecuteScript(string querySql) => throw new NotSupportedException();

        public override Task<int> ExecuteScriptAsync(string querySql, CancellationToken? cancellationToken = null) => throw new NotSupportedException();

        public override object GetLastInsertID() => throw new NotSupportedException();

        public override Task<object> GetLastInsertIdAsync() => throw new NotSupportedException();

        public override bool CheckIfTableExists(string tableName) => throw new NotSupportedException();

        public override Task<bool> CheckIfTableExistsAsync(string tableName) => throw new NotSupportedException();
    }

    private sealed class RenderingLanguageFactory : LanguageFactory
    {
        public override bool GroupBySupportsOrdering => true;

        public override void BuildLimitOffset(Query query, bool top, StringBuilder outputBuilder)
        {
            if (!top && query.Limit > 0)
            {
                outputBuilder.Append($" LIMIT {query.Limit}");
                if (query.Offset > 0)
                    outputBuilder.Append($" OFFSET {query.Offset}");
            }
        }

        public override void BuildColumnPropertiesDataType(
            TableSchema.Column column,
            out bool isDefaultAllowed,
            StringBuilder sb,
            ConnectorBase connection,
            Query relatedQuery)
        {
            isDefaultAllowed = true;
            sb.Append(column.ActualDataType == DataType.Int ? "INTEGER" : "TEXT");
        }

        public override (string typeString, bool isDefaultAllowed) BuildDataTypeDef(DataTypeDef typeDef, bool forCast = false)
        {
            return (typeDef.Type == DataType.Int ? "INTEGER" : "TEXT", true);
        }
    }
}
