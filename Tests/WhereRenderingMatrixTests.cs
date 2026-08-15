using System.Text;
using SequelNet;
using SequelNet.Connector;

namespace Tests;

public class WhereRenderingMatrixTests
{
    [TestCase(1, "`id` = 5")]
    [TestCase(2, "`id` IS NULL")]
    [TestCase(3, "`id` <> 5")]
    [TestCase(4, "`id` IS NOT NULL")]
    [TestCase(5, "`id` > 5")]
    [TestCase(6, "`id` >= 5")]
    [TestCase(7, "`id` < 5")]
    [TestCase(8, "`id` <= 5")]
    [TestCase(9, "`id` IS 5")]
    [TestCase(10, "`id` IS NOT 5")]
    [TestCase(11, "`id` LIKE 'a%' ESCAPE('\\') ")]
    [TestCase(12, "`id` NOT LIKE 'a%' ESCAPE('\\') ")]
    [TestCase(13, "`id` BETWEEN 1 AND 10")]
    [TestCase(14, "`id` IN (1,2,3)")]
    [TestCase(15, "`id` NOT IN (1,2,3)")]
    [TestCase(16, " 0 ")]
    [TestCase(17, " 1 ")]
    [TestCase(18, "TRUE")]
    [TestCase(19, "1")]
    [TestCase(20, "(`id` = 5)")]
    [TestCase(21, "`orders`.`id` = 5")]
    [TestCase(22, "`id` = `orders`.`other_id`")]
    [TestCase(23, "`orders`.`id` = `customers`.`id`")]
    [TestCase(24, "`id` IN ('a','O''Brian')")]
    [TestCase(25, "(`id` = 1 AND `enabled` = 1)")]
    public void Where_BuildsExpectedSqlForEachComparisonShape(int scenario, string expected)
    {
        var where = CreateScenario(scenario);

        Assert.That(Render(where), Is.EqualTo(expected));
    }

    private static Where CreateScenario(int scenario)
    {
        return scenario switch
        {
            1 => Comparison(WhereComparison.EqualsTo, 5),
            2 => Comparison(WhereComparison.EqualsTo, null),
            3 => Comparison(WhereComparison.NotEqualsTo, 5),
            4 => Comparison(WhereComparison.NotEqualsTo, null),
            5 => Comparison(WhereComparison.GreaterThan, 5),
            6 => Comparison(WhereComparison.GreaterThanOrEqual, 5),
            7 => Comparison(WhereComparison.LessThan, 5),
            8 => Comparison(WhereComparison.LessThanOrEqual, 5),
            9 => Comparison(WhereComparison.Is, 5),
            10 => Comparison(WhereComparison.IsNot, 5),
            11 => Comparison(WhereComparison.Like, "a%"),
            12 => Comparison(WhereComparison.NotLike, "a%"),
            13 => new Where(WhereCondition.AND, "id", ValueObjectType.ColumnName, 1, ValueObjectType.Value, 10, ValueObjectType.Value),
            14 => Comparison(WhereComparison.In, new List<int> { 1, 2, 3 }),
            15 => Comparison(WhereComparison.NotIn, new List<int> { 1, 2, 3 }),
            16 => Comparison(WhereComparison.In, new List<int>()),
            17 => Comparison(WhereComparison.NotIn, new List<int>()),
            18 => new Where(WhereCondition.AND, "TRUE", ValueObjectType.Literal, WhereComparison.None, null, ValueObjectType.Value),
            19 => new Where(new WhereList()),
            20 => new Where(new WhereList().Where("id", 5)),
            21 => new Where("orders", "id", WhereComparison.EqualsTo, 5),
            22 => new Where("id", ValueObjectType.ColumnName, WhereComparison.EqualsTo, "other_id", ValueObjectType.ColumnName)
            {
                SecondTableName = "orders"
            },
            23 => new Where("orders", "id", WhereComparison.EqualsTo, "customers", "id"),
            24 => Comparison(WhereComparison.In, new List<string> { "a", "O'Brian" }),
            25 => new Where(new WhereList().Where("id", 1).AND("enabled", true)),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
    }

    private static Where Comparison(WhereComparison comparison, object? value)
    {
        return new Where(WhereCondition.AND, "id", ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value);
    }

    private static string Render(Where where)
    {
        var output = new StringBuilder();
        where.BuildCommand(output, true, new Where.BuildContext { Conn = new TestWhereConnector() });
        return output.ToString();
    }

    private sealed class TestWhereConnector : ConnectorBase
    {
        public override IConnectorFactory Factory => throw new NotSupportedException();

        public override int ExecuteScript(string querySql) => throw new NotSupportedException();

        public override Task<int> ExecuteScriptAsync(string querySql, CancellationToken? cancellationToken = null) => throw new NotSupportedException();

        public override object GetLastInsertID() => throw new NotSupportedException();

        public override Task<object> GetLastInsertIdAsync() => throw new NotSupportedException();

        public override bool CheckIfTableExists(string tableName) => throw new NotSupportedException();

        public override Task<bool> CheckIfTableExistsAsync(string tableName) => throw new NotSupportedException();
    }
}
