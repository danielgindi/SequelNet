using System.Text;
using SequelNet;
using SequelNet.Connector;

namespace Tests;

public class SqlRenderingTests
{
    [Test]
    public void Query_BuildsSelectWhereAndOrderByWithEscapedIdentifiers()
    {
        var query = new Query("orders", "o")
            .Select("o", "id", null)
            .SelectLiteral("COUNT(*)", "count")
            .Where("o", "status", WhereComparison.EqualsTo, "active")
            .AND("o", "total", WhereComparison.GreaterThan, 10)
            .OrderBy("o", "id", SortDirection.DESC);

        var sql = query.BuildCommand(new TestSqlConnector());

        Assert.That(sql, Is.EqualTo(
            " SELECT `o`.`id`,COUNT(*) AS `count` FROM `orders` `o` WHERE `o`.`status` = 'active' AND `o`.`total` > 10 ORDER BY `o`.`id` DESC"));
    }

    [Test]
    public void Query_BuildsNestedWhereAndBetweenConditions()
    {
        var nested = new WhereList().Where("is_deleted", false).OR("is_deleted", null);
        var query = new Query("orders")
            .SelectAll()
            .Where(nested)
            .AND("created_at", ValueObjectType.ColumnName, ValueWrapper.From("2024-01-01"), ValueObjectType.Value, ValueWrapper.From("2024-12-31"), ValueObjectType.Value);

        var sql = query.BuildCommand(new TestSqlConnector());

        Assert.That(sql, Is.EqualTo(
            " SELECT * FROM `orders` WHERE (`is_deleted` = 0 OR `is_deleted` IS NULL) AND `created_at` BETWEEN '2024-01-01' AND '2024-12-31'"));
    }

    [Test]
    public void Phrases_RenderColumnsValuesAndNullHandling()
    {
        var connector = new TestSqlConnector();
        var expression = PhraseHelper.Concat(
            true,
            ValueWrapper.From("prefix-"),
            ValueWrapper.Column("orders", "reference"),
            ValueWrapper.From("-suffix"));
        var aggregate = PhraseHelper.Count("orders", "id", distinct: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Build(PhraseHelper.Add(ValueWrapper.Column("orders", "total"), ValueWrapper.From(5)), connector),
                Is.EqualTo("`orders`.`total` + 5"));
            Assert.That(Build(expression, connector),
                Is.EqualTo("CONCAT(COALESCE('prefix-',''),COALESCE(`orders`.`reference`,''),COALESCE('-suffix',''))"));
            Assert.That(Build(aggregate, connector), Is.EqualTo("COUNT(DISTINCT `orders`.`id`)"));
        }
    }

    [Test]
    public void LanguageFactory_PreparesEscapedValuesAndForeignKeyActions()
    {
        var language = new LanguageFactory();
        var foreignKey = new TableSchema.ForeignKey(
            "FK_orders_customer", "customer_id", "customers", "id",
            TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.SetNull);
        var sql = new StringBuilder();

        language.BuildCreateForeignKey(foreignKey, sql, new TestSqlConnector());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(language.WrapFieldName("weird`name"), Is.EqualTo("`weird``name`"));
            Assert.That(language.PrepareValue("O'Brian"), Is.EqualTo("'O''Brian'"));
            Assert.That(language.FormatBinary(new byte[] { 0x01, 0xAB }), Is.EqualTo("UNHEX(01AB)"));
            Assert.That(sql.ToString(), Is.EqualTo("`FK_orders_customer` FOREIGN KEY (`customer_id`) REFERENCES customers (`id`) ON DELETE CASCADE ON UPDATE SET NULL"));
        }
    }

    [Test]
    public void OrderByList_ThenWithTableKeepsTheTableName()
    {
        var orderBy = new OrderBy("first_column", SortDirection.ASC);

        var list = orderBy.Then("orders", "second_column", SortDirection.DESC);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[1].Value.TableName, Is.EqualTo("orders"));
            Assert.That(list[1].Value.Value, Is.EqualTo("second_column"));
        }
    }

    private static string Build(IPhrase phrase, ConnectorBase connector)
    {
        var builder = new StringBuilder();
        phrase.Build(builder, connector);
        return builder.ToString();
    }

    private sealed class TestSqlConnector : ConnectorBase
    {
        private static readonly LanguageFactory TestLanguage = new TestLanguageFactory();

        public override IConnectorFactory Factory => throw new NotSupportedException();

        public override LanguageFactory Language => TestLanguage;

        public override int ExecuteScript(string querySql) => throw new NotSupportedException();

        public override Task<int> ExecuteScriptAsync(string querySql, CancellationToken? cancellationToken = null) =>
            throw new NotSupportedException();

        public override object GetLastInsertID() => throw new NotSupportedException();

        public override Task<object> GetLastInsertIdAsync() => throw new NotSupportedException();

        public override bool CheckIfTableExists(string tableName) => throw new NotSupportedException();

        public override Task<bool> CheckIfTableExistsAsync(string tableName) => throw new NotSupportedException();
    }

    private sealed class TestLanguageFactory : LanguageFactory
    {
        public override void BuildLimitOffset(Query query, bool top, StringBuilder outputBuilder)
        {
            if (!top && query.Limit > 0)
            {
                outputBuilder.Append($" LIMIT {query.Limit}");
                if (query.Offset > 0)
                    outputBuilder.Append($" OFFSET {query.Offset}");
            }
        }
    }
}
