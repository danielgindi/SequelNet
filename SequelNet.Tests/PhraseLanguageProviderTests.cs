using System.Text;
using SequelNet;
using SequelNet.Connector;
using SequelNet.Phrases;

namespace Tests;

public class PhraseLanguageProviderTests
{
    private static IEnumerable<TestCaseData> Dialects()
    {
        yield return DialectCase(
            "MySQL_MySqlData",
            ConnectorBase.SqlServiceType.MYSQL,
            new MySqlLanguageFactory(new MySqlMode { Version = "8.0.21" }));
        yield return DialectCase(
            "MySQL_MySqlConnector",
            ConnectorBase.SqlServiceType.MYSQL,
            new MySql2LanguageFactory(new MySql2Mode { Version = "8.0.21" }));
        yield return DialectCase(
            "PostgreSQL",
            ConnectorBase.SqlServiceType.POSTGRESQL,
            new PostgreSQLLanguageFactory(new PostgreSQLMode { Version = "16.0" }));
        yield return DialectCase(
            "MsSql",
            ConnectorBase.SqlServiceType.MSSQL,
            new MsSqlLanguageFactory(new MsSqlVersion { Version = "16.0" }));
        yield return DialectCase(
            "OleDb",
            ConnectorBase.SqlServiceType.MSACCESS,
            new OleDbLanguageFactory());
    }

    [TestCaseSource(nameof(Dialects))]
    public void DialectAwarePhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                Build(new Concat(false, Literal("left"), Literal("right")), connector),
                Is.EqualTo(dialect.Concat),
                "Concat");
            Assert.That(
                Build(new Concat(true, Literal("left"), Literal("right")), connector),
                Is.EqualTo(dialect.ConcatIgnoringNulls),
                "Concat ignoring nulls");
            Assert.That(
                Build(new Concat(), connector),
                Is.EqualTo(dialect.EmptyConcat),
                "Empty Concat");
            Assert.That(
                Build(new Concat(false, Literal("value")), connector),
                Is.EqualTo(dialect.SingleConcat),
                "Single-value Concat");
            Assert.That(
                Build(new Concat(true, Literal("value")), connector),
                Is.EqualTo(dialect.SingleConcatIgnoringNulls),
                "Single-value Concat ignoring nulls");
            Assert.That(
                Build(new RandWeight("weight", ValueObjectType.Literal), connector),
                Is.EqualTo(dialect.RandWeight),
                "RandWeight");
            if (dialect.Type == ConnectorBase.SqlServiceType.MSACCESS)
            {
                AssertNotSupported(
                    new DateTimeAdd(
                        "date_value", ValueObjectType.Literal,
                        DateTimeUnit.Millisecond,
                        2),
                    "millisecond",
                    connector);
                AssertNotSupported(
                    new DateTimeDiff(
                        DateTimeUnit.Millisecond,
                        "earlier", ValueObjectType.Literal,
                        "later", ValueObjectType.Literal),
                    "millisecond",
                    connector);
            }
            else
            {
                Assert.That(
                    Build(new DateTimeAdd(
                        "date_value", ValueObjectType.Literal,
                        DateTimeUnit.Millisecond,
                        2), connector),
                    Is.EqualTo(dialect.DateTimeAdd),
                    "DateTimeAdd");
                Assert.That(
                    Build(new DateTimeDiff(
                        DateTimeUnit.Millisecond,
                        "earlier", ValueObjectType.Literal,
                        "later", ValueObjectType.Literal), connector),
                    Is.EqualTo(dialect.DateTimeDiff),
                    "DateTimeDiff");
            }
        }
    }

    [Test]
    public void Concat_RejectsUnsupportedSqlServerVersions()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.MSSQL,
            new MsSqlLanguageFactory(new MsSqlVersion { Version = "10.0" }));

        AssertNotSupported(
            new Concat(false, Literal("left"), Literal("right")),
            "SQL Server 2012",
            connector);
    }

    [Test]
    public void PostgreSqlDateTimeDiff_UsesElapsedTimeAndThreeMonthQuarters()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.POSTGRESQL,
            new PostgreSQLLanguageFactory(new PostgreSQLMode { Version = "16.0" }));

        using (Assert.EnterMultipleScope())
        {
            AssertSql(
                new DateTimeDiff(
                    DateTimeUnit.Day,
                    "earlier", ValueObjectType.Literal,
                    "later", ValueObjectType.Literal),
                "TRUNC(EXTRACT(EPOCH FROM (later - earlier)) / 86400)",
                connector);
            AssertSql(
                new DateTimeDiff(
                    DateTimeUnit.QuarterYear,
                    "earlier", ValueObjectType.Literal,
                    "later", ValueObjectType.Literal),
                "(DATE_PART('year', later) - DATE_PART('year', earlier)) * 4 + " +
                "(DATE_PART('quarter', later) - DATE_PART('quarter', earlier))",
                connector);
        }
    }

    [Test]
    public void PostgreSqlDateTimeAdd_MultipliesTypedIntervals()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.POSTGRESQL,
            new PostgreSQLLanguageFactory(new PostgreSQLMode { Version = "16.0" }));

        using (Assert.EnterMultipleScope())
        {
            AssertSql(
                new DateTimeAdd(Literal("date_value"), DateTimeUnit.Day, Literal("amount")),
                "date_value + (amount * INTERVAL '1 day')",
                connector);
            AssertSql(
                new DateTimeAdd(
                    "date_value", ValueObjectType.Literal,
                    DateTimeUnit.Microsecond,
                    2),
                "date_value + (2 * INTERVAL '1 microsecond')",
                connector);
            AssertSql(
                new DateTimeAdd(
                    "date_value", ValueObjectType.Literal,
                    DateTimeUnit.QuarterYear,
                    2),
                "date_value + (2 * INTERVAL '3 months')",
                connector);
        }
    }

    [Test]
    public void OleDbDateTimePhrases_UseAccessIntervalsForSupportedUnits()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.MSACCESS,
            new OleDbLanguageFactory());

        using (Assert.EnterMultipleScope())
        {
            AssertSql(
                new DateTimeAdd(
                    "date_value", ValueObjectType.Literal,
                    DateTimeUnit.Day,
                    2),
                "DATEADD('d', (2), date_value)",
                connector);
            AssertSql(
                new DateTimeDiff(
                    DateTimeUnit.Day,
                    "earlier", ValueObjectType.Literal,
                    "later", ValueObjectType.Literal),
                "DATEDIFF('d', earlier, later)",
                connector);
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void MathPhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);

        using (Assert.EnterMultipleScope())
        {
            AssertSql(new Abs("value", ValueObjectType.Literal), "ABS(value)", connector);
            AssertSql(new Add(Literal("left"), Literal("right")), "(left + right)", connector);
            AssertSql(
                new Ceil("value", ValueObjectType.Literal),
                dialect.Type switch
                {
                    ConnectorBase.SqlServiceType.MSSQL => "CEILING(value)",
                    ConnectorBase.SqlServiceType.MSACCESS => "(-INT(-(value)))",
                    _ => "CEIL(value)",
                },
                connector);
            AssertSql(
                new Divide("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                "(left / right)",
                connector);
            AssertSql(
                new Floor("value", ValueObjectType.Literal),
                dialect.Type == ConnectorBase.SqlServiceType.MSACCESS ? "INT(value)" : "FLOOR(value)",
                connector);

            if (dialect.Type == ConnectorBase.SqlServiceType.MSACCESS)
            {
                AssertNotImplemented(
                    new Greatest("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                    "GREATEST",
                    connector);
                AssertNotImplemented(
                    new Least("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                    "LEAST",
                    connector);
            }
            else
            {
                AssertSql(
                    new Greatest("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                    "GREATEST(left, right)",
                    connector);
                AssertSql(
                    new Least("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                    "LEAST(left, right)",
                    connector);
            }
            AssertSql(
                new Multiply("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                "(left * right)",
                connector);
            AssertSql(new Round("value", ValueObjectType.Literal, 2), "ROUND(value,2)", connector);
            AssertSql(
                new Round("value", ValueObjectType.Literal),
                dialect.Type == ConnectorBase.SqlServiceType.MSSQL
                    ? "ROUND(value,0)"
                    : "ROUND(value)",
                connector);
            AssertSql(
                new Subtract("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                "(left-right)",
                connector);
            AssertSql(
                new Multiply(new Add(Literal("left"), Literal("right")), 2),
                "((left + right) * 2)",
                connector);
            AssertSql(
                new Divide(2, new Add(Literal("left"), Literal("right"))),
                "(2 / (left + right))",
                connector);

            Action emptyAdd = () => Build(new Add(), connector);
            Assert.That(
                Assert.Throws<InvalidOperationException>(emptyAdd)?.Message,
                Does.Contain("at least one value"));
        }
    }

    [Test]
    public void GreatestAndLeast_RequireSqlServer2022OrLater()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.MSSQL,
            new MsSqlLanguageFactory(new MsSqlVersion { Version = "15.0" }));

        using (Assert.EnterMultipleScope())
        {
            AssertNotSupported(
                new Greatest("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                "SQL Server 2022",
                connector);
            AssertNotSupported(
                new Least("left", ValueObjectType.Literal, "right", ValueObjectType.Literal),
                "SQL Server 2022",
                connector);
        }
    }

    [Test]
    public void SqlServerPhrases_RejectVersionsBeforeFeatureIntroduction()
    {
        var sqlServer2008 = new TestConnector(
            ConnectorBase.SqlServiceType.MSSQL,
            new MsSqlLanguageFactory(new MsSqlVersion { Version = "10.0" }));
        var sqlServer2014 = new TestConnector(
            ConnectorBase.SqlServiceType.MSSQL,
            new MsSqlLanguageFactory(new MsSqlVersion { Version = "12.0" }));

        using (Assert.EnterMultipleScope())
        {
            AssertNotSupported(
                new DateTimeFormat(Literal("value"), DateTimeFormat.FormatOptions.IsoDate),
                "SQL Server 2012",
                sqlServer2008);
            AssertNotSupported(
                new ConvertUtcToTz(Literal("value"), Literal("timezone")),
                "SQL Server 2016",
                sqlServer2014);
            AssertNotSupported(
                new JsonExtract("doc", ValueObjectType.Literal, "$.key", true),
                "SQL Server 2016",
                sqlServer2014);
            AssertNotSupported(
                new JsonValue("doc", ValueObjectType.Literal, "$.key"),
                "SQL Server 2016",
                sqlServer2014);
        }
    }

    [Test]
    public void SqlServerConcat_RejectsMoreThan254Arguments()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.MSSQL,
            new MsSqlLanguageFactory(new MsSqlVersion { Version = "16.0" }));
        var maximumArguments = Enumerable.Range(0, 254)
            .Select(index => Literal($"value{index}"))
            .ToArray();
        var tooManyArguments = maximumArguments
            .Append(Literal("overflow"))
            .ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                Build(new Concat(maximumArguments), connector),
                Does.Contain("CONCAT("));
            AssertNotSupported(
                new Concat(tooManyArguments),
                "254",
                connector);
        }
    }

    [Test]
    public void SqlServerConcat_EvaluatesEachOperandOnce()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.MSSQL,
            new MsSqlLanguageFactory(new MsSqlVersion { Version = "16.0" }));
        var sql = Build(
            new Concat(false, Literal("volatile_value()")),
            connector);

        Assert.That(
            CountOccurrences(sql, "volatile_value()"),
            Is.EqualTo(1));
    }

    [TestCaseSource(nameof(Dialects))]
    public void AggregateWildcard_IsLimitedToNonDistinctCount(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var invalidAggregates = new BaseAggregatePhrase[]
        {
            new Avg(),
            new Max(),
            new Min(),
            new Sum(),
            new Some(),
            new Every(),
            new StandardDeviationOfPopulation(),
            new StandardDeviationOfSample(),
            new StandardVarianceOfPopulation(),
            new StandardVarianceOfSample(),
            new Count(true),
            new CountDistinct(),
        };

        using (Assert.EnterMultipleScope())
        {
            foreach (var aggregate in invalidAggregates)
                AssertInvalidOperation(() => Build(aggregate, connector));

            AssertSql(new Count(), "COUNT(*)", connector);

            var populatedAggregate = new Avg { Value = Literal("value") };
            AssertSql(populatedAggregate, "AVG(value)", connector);
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void AggregatePhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);

        using (Assert.EnterMultipleScope())
        {
            AssertSql(new Avg("value", ValueObjectType.Literal), "AVG(value)", connector);
            AssertSql(new Count("value", ValueObjectType.Literal), "COUNT(value)", connector);
            AssertSql(new Max("value", ValueObjectType.Literal), "MAX(value)", connector);
            AssertSql(new Min("value", ValueObjectType.Literal), "MIN(value)", connector);
            AssertSql(new Sum("value", ValueObjectType.Literal), "SUM(value)", connector);

            if (dialect.Type == ConnectorBase.SqlServiceType.MSACCESS)
            {
                AssertNotImplemented(
                    new Count("value", ValueObjectType.Literal, true),
                    "DISTINCT aggregate",
                    connector);
                AssertNotImplemented(
                    new CountDistinct("value", ValueObjectType.Literal),
                    "DISTINCT aggregate",
                    connector);
                AssertNotImplemented(
                    new Max("value", ValueObjectType.Literal, true),
                    "DISTINCT aggregate",
                    connector);
                AssertNotImplemented(
                    new Min("value", ValueObjectType.Literal, true),
                    "DISTINCT aggregate",
                    connector);
                AssertNotImplemented(
                    new Sum("value", ValueObjectType.Literal, true),
                    "DISTINCT aggregate",
                    connector);
            }
            else
            {
                AssertSql(new Count("value", ValueObjectType.Literal, true), "COUNT(DISTINCT value)", connector);
                AssertSql(new CountDistinct("value", ValueObjectType.Literal), "COUNT(DISTINCT value)", connector);
                AssertSql(new Max("value", ValueObjectType.Literal, true), "MAX(DISTINCT value)", connector);
                AssertSql(new Min("value", ValueObjectType.Literal, true), "MIN(DISTINCT value)", connector);
                AssertSql(new Sum("value", ValueObjectType.Literal, true), "SUM(DISTINCT value)", connector);
            }
            AssertSql(new PassThroughAggregate("CUSTOM", "value", ValueObjectType.Literal), "CUSTOM(value)", connector);
            var populationDeviation = dialect.Type switch
            {
                ConnectorBase.SqlServiceType.MSSQL => "STDEVP(value)",
                ConnectorBase.SqlServiceType.MSACCESS => "StDevP(value)",
                _ => "STDDEV_POP(value)",
            };
            var sampleDeviation = dialect.Type switch
            {
                ConnectorBase.SqlServiceType.MSSQL => "STDEV(value)",
                ConnectorBase.SqlServiceType.MSACCESS => "StDev(value)",
                _ => "STDDEV_SAMP(value)",
            };
            var populationVariance = dialect.Type switch
            {
                ConnectorBase.SqlServiceType.MSSQL => "VARP(value)",
                ConnectorBase.SqlServiceType.MSACCESS => "VarP(value)",
                _ => "VAR_POP(value)",
            };
            var sampleVariance = dialect.Type switch
            {
                ConnectorBase.SqlServiceType.MSSQL => "VAR(value)",
                ConnectorBase.SqlServiceType.MSACCESS => "Var(value)",
                _ => "VAR_SAMP(value)",
            };
            AssertSql(
                new StandardDeviationOfPopulation("value", ValueObjectType.Literal),
                populationDeviation,
                connector);
            AssertSql(
                new StandardDeviationOfSample("value", ValueObjectType.Literal),
                sampleDeviation,
                connector);
            AssertSql(
                new StandardVarianceOfPopulation("value", ValueObjectType.Literal),
                populationVariance,
                connector);
            AssertSql(
                new StandardVarianceOfSample("value", ValueObjectType.Literal),
                sampleVariance,
                connector);
        }

        if (dialect.Type == ConnectorBase.SqlServiceType.MYSQL)
        {
            AssertSql(new Some("value", ValueObjectType.Literal), "(SUM(value) > 0)", connector);
            AssertSql(new Every("value", ValueObjectType.Literal), "(BIT_AND(value) > 0)", connector);
            AssertSql(
                new GroupConcat(true, Literal("value"), ";"),
                "GROUP_CONCAT(DISTINCT value SEPARATOR ';')",
                connector);
            return;
        }

        if (dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL)
        {
            AssertSql(new Some("value", ValueObjectType.Literal), "bool_or(value)", connector);
            AssertSql(new Every("value", ValueObjectType.Literal), "bool_and(value)", connector);
            AssertSql(
                new GroupConcat(true, Literal("value"), ";"),
                "string_agg(DISTINCT value,';')",
                connector);
            return;
        }

        if (dialect.Type == ConnectorBase.SqlServiceType.MSSQL)
        {
            AssertSql(new Some("value", ValueObjectType.Literal), "(MAX(CAST(value AS INT)) > 0)", connector);
            AssertSql(
                new Every("value", ValueObjectType.Literal),
                "(MIN(CAST(value AS INT)) > 0)",
                connector);
            AssertNotImplemented(new GroupConcat(Literal("value")), "GROUP_CONCAT", connector);
            return;
        }

        AssertNotImplemented(new Some("value", ValueObjectType.Literal), "SOME", connector);
        AssertNotImplemented(new Every("value", ValueObjectType.Literal), "EVERY", connector);
        AssertNotImplemented(new GroupConcat(Literal("value")), "GROUP_CONCAT", connector);
    }

    [TestCaseSource(nameof(Dialects))]
    public void StringPhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var isOleDb = dialect.Type == ConnectorBase.SqlServiceType.MSACCESS;
        var isMsSql = dialect.Type == ConnectorBase.SqlServiceType.MSSQL;
        var isPostgreSql = dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL;

        using (Assert.EnterMultipleScope())
        {
            AssertSql(
                new FindString(Literal("needle"), Literal("haystack")),
                isPostgreSql
                    ? "POSITION(needle IN haystack)"
                    : isOleDb
                        ? "InStr(haystack,needle)"
                        : isMsSql
                            ? "CHARINDEX(needle,haystack)"
                            : "LOCATE(needle,haystack)",
                connector);
            AssertSql(
                new Length("value", ValueObjectType.Literal),
                isOleDb || isMsSql
                    ? "LEN(value)"
                    : dialect.Type == ConnectorBase.SqlServiceType.MYSQL
                        ? "CHAR_LENGTH(value)"
                        : "LENGTH(value)",
                connector);
            AssertSql(
                new Lower("value", ValueObjectType.Literal),
                isOleDb ? "LCASE(value)" : "LOWER(value)",
                connector);
            AssertSql(
                new Replace(Literal("source"), Literal("search"), Literal("replacement")),
                "REPLACE(source,search,replacement)",
                connector);
            AssertSql(
                new Substring("value", ValueObjectType.Literal, 2, 3),
                isOleDb
                    ? "MID(value, 2, 3)"
                    : isMsSql
                        ? "SUBSTRING(value, 2, 3)"
                        : "SUBSTRING(value FROM 2 FOR 3)",
                connector);
            AssertSql(
                new Upper("value", ValueObjectType.Literal),
                isOleDb ? "UCASE(value)" : "UPPER(value)",
                connector);
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void DateTimePhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var isMySql = dialect.Type == ConnectorBase.SqlServiceType.MYSQL;
        var isPostgreSql = dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL;
        var isMsSql = dialect.Type == ConnectorBase.SqlServiceType.MSSQL;
        var isOleDb = dialect.Type == ConnectorBase.SqlServiceType.MSACCESS;

        using (Assert.EnterMultipleScope())
        {
            if (isOleDb)
            {
                AssertNotImplemented(
                    new ConvertUtcToTz(Literal("value"), Literal("timezone")),
                    "UTC time zone conversion",
                    connector);
            }
            else
            {
                AssertSql(
                    new ConvertUtcToTz(Literal("value"), Literal("timezone")),
                    isMySql
                        ? "CONVERT_TZ(value,'UTC',timezone)"
                        : "value AT TIME ZONE 'UTC' AT TIME ZONE timezone",
                    connector);
            }
            AssertSql(
                new CreateDate(2026, 9, 7),
                isMsSql
                    ? "CAST('2026-09-07' AS DATE)"
                    : isOleDb ? "DATESERIAL(2026, 9, 7)" : "DATE'2026-09-07'",
                connector);
            AssertSql(
                new CreateTime(4, 5, 6, 7),
                isMsSql
                    ? "CAST('04:05:06.007' AS TIME)"
                    : isOleDb
                        ? "TIMESERIAL(4, 5, 6) + (7 / 86400000.0)"
                        : "TIME'04:05:06.007'",
                connector);
            foreach (var (format, expected) in DateTimeFormatsFor(dialect.Type))
            {
                var phrase = new DateTimeFormat("value", ValueObjectType.Literal, format);
                if (expected == null)
                    AssertNotImplemented(phrase, "DateTimeFormat with format", connector);
                else
                    AssertSql(phrase, expected, connector);
            }
            AssertSql(
                new Year("value", ValueObjectType.Literal),
                isPostgreSql ? "EXTRACT(YEAR FROM value)" : "YEAR(value)",
                connector);
            AssertSql(
                new Month("value", ValueObjectType.Literal),
                isPostgreSql ? "EXTRACT(MONTH FROM value)" : "MONTH(value)",
                connector);
            AssertSql(
                new Day("value", ValueObjectType.Literal),
                isPostgreSql ? "EXTRACT(DAY FROM value)" : "DAY(value)",
                connector);
            AssertSql(
                new Hour("value", ValueObjectType.Literal),
                isPostgreSql
                    ? "EXTRACT(HOUR FROM value)"
                    : isMsSql ? "DATEPART(hour, value)" : "HOUR(value)",
                connector);
            AssertSql(
                new Minute("value", ValueObjectType.Literal),
                isPostgreSql
                    ? "EXTRACT(MINUTE FROM value)"
                    : isMsSql ? "DATEPART(minute, value)" : "MINUTE(value)",
                connector);
            AssertSql(
                new Second("value", ValueObjectType.Literal),
                isPostgreSql
                    ? "EXTRACT(SECOND FROM value)"
                    : isMsSql ? "DATEPART(second, value)" : "SECOND(value)",
                connector);
            AssertSql(
                new ExtractDate("value", ValueObjectType.Literal),
                isPostgreSql
                    ? "value::DATE"
                    : isMsSql
                        ? "CAST(value AS DATE)"
                        : isOleDb ? "DATEVALUE(value)" : "DATE(value)",
                connector);
            AssertSql(
                new ExtractTime("value", ValueObjectType.Literal),
                isPostgreSql
                    ? "value::TIME"
                    : isMsSql
                        ? "CAST(value AS TIME)"
                        : isOleDb ? "TIMEVALUE(value)" : "TIME(value)",
                connector);
            AssertSql(
                new UnixTimestamp("value", ValueObjectType.Literal),
                isPostgreSql
                    ? "EXTRACT(epoch FROM value)"
                    : isMsSql
                        ? "DATEDIFF(second, '1970-01-01 00:00:00', value)"
                        : isOleDb
                            ? "DATEDIFF('s', #1970-01-01 00:00:00#, value)"
                            : "UNIX_TIMESTAMP(value)",
                connector);

            var expectedUtcNow = isMySql
                ? "UTC_TIMESTAMP()"
                : isPostgreSql
                    ? "now() at time zone 'utc'"
                    : "GETUTCDATE()";

            if (isOleDb)
                AssertNotImplemented(new UtcTimestamp(), "UTC timestamp", connector);
            else
                AssertSql(new UtcTimestamp(), expectedUtcNow, connector);
#pragma warning disable CS0618
            if (isOleDb)
                AssertNotImplemented(new UTC_TIMESTAMP(), "UTC timestamp", connector);
            else
                AssertSql(new UTC_TIMESTAMP(), expectedUtcNow, connector);
#pragma warning restore CS0618
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void FlowControlPhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var isNullFunction = dialect.Type switch
        {
            ConnectorBase.SqlServiceType.MSSQL => "ISNULL",
            ConnectorBase.SqlServiceType.MSACCESS => "Nz",
            ConnectorBase.SqlServiceType.POSTGRESQL => "COALESCE",
            _ => "IFNULL",
        };
        var casePhrase = new Case(Literal("subject"))
            .When(Literal("choice"))
            .Then(Literal("result"))
            .Else(Literal("fallback"));

        using (Assert.EnterMultipleScope())
        {
            if (dialect.Type == ConnectorBase.SqlServiceType.MSACCESS)
                AssertNotImplemented(casePhrase, "CASE", connector);
            else
            {
                AssertSql(
                    casePhrase,
                    "CASE subject WHEN choice THEN result ELSE fallback END",
                    connector);
                Action emptyCase = () => Build(new Case(), connector);
                Assert.That(
                    Assert.Throws<InvalidOperationException>(emptyCase)?.Message,
                    Does.Contain("at least one WHEN condition"));
            }
            AssertSql(
                new IfNull(Literal("value"), Literal("fallback")),
                $"{isNullFunction}(value, fallback)",
                connector);
            AssertSql(
                new NullIf(Literal("left"), Literal("right")),
                dialect.Type == ConnectorBase.SqlServiceType.MSACCESS
                    ? "IIF(left = right, NULL, left)"
                    : "NULLIF(left, right)",
                connector);
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void GeneralPhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var isPostgreSql = dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL;
        var firstQuery = new Query("first_items").SelectLiteral("1");
        var secondQuery = new Query("second_items").SelectLiteral("2");
        var firstSql = firstQuery.BuildCommand(connector);
        var secondSql = secondQuery.BuildCommand(connector);
        var castType = dialect.Type switch
        {
            ConnectorBase.SqlServiceType.MYSQL => "SIGNED",
            ConnectorBase.SqlServiceType.POSTGRESQL => "BOOLEAN",
            ConnectorBase.SqlServiceType.MSSQL => "bit",
            _ => "BIT",
        };

        using (Assert.EnterMultipleScope())
        {
            if (dialect.Type == ConnectorBase.SqlServiceType.MSACCESS)
            {
                AssertNotImplemented(
                    new Cast(Literal("value"), new DataTypeDef { Type = DataType.Boolean }),
                    "CAST",
                    connector);
                AssertNotImplemented(
                    new Collate(Literal("value"), "test_collation", SortDirection.DESC),
                    "COLLATE",
                    connector);
            }
            else
            {
                AssertSql(
                    new Cast(Literal("value"), new DataTypeDef { Type = DataType.Boolean }),
                    $"CAST(value AS {castType})",
                    connector);
                AssertSql(
                    new Collate(Literal("value"), "test_collation", SortDirection.DESC),
                    dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL
                        ? "(value COLLATE \"test_collation\")"
                        : "(value COLLATE test_collation)",
                    connector);
            }
            AssertSql(new Exists(firstQuery), $"EXISTS ({firstSql})", connector);
            AssertSql(new NotExists(firstQuery), $"NOT EXISTS ({firstSql})", connector);
            AssertSql(
                new Literal((builder, _, _) => builder.Append("literal_sql")),
                "literal_sql",
                connector);
            AssertSql(
                new Union(firstQuery, secondQuery),
                $"({firstSql} UNION{secondSql})",
                connector);
            AssertSql(
                new UnionAll(firstQuery, secondQuery),
                $"({firstSql} UNION ALL{secondSql})",
                connector);

            Action emptyUnion = () => Build(new Union(), connector);
            Assert.That(
                Assert.Throws<InvalidOperationException>(emptyUnion)?.Message,
                Does.Contain("at least one query"));
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void ConflictColumnPhrase_RendersExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var schema = new TableSchema("items", null);
        schema.AddColumn(new TableSchema.Column { Name = "value", Type = typeof(int) });
        var insertQuery = new Query(schema).Insert("value", 7);
        var phrase = new ConflictColumn("value");

        switch (dialect.Type)
        {
            case ConnectorBase.SqlServiceType.MYSQL:
                Assert.That(Build(phrase, connector, insertQuery), Is.EqualTo("VALUES(`value`)"));
                break;

            case ConnectorBase.SqlServiceType.POSTGRESQL:
                Assert.That(Build(phrase, connector, insertQuery), Is.EqualTo("EXCLUDED.\"value\""));
                break;

            case ConnectorBase.SqlServiceType.MSSQL:
                Assert.That(Build(phrase, connector, insertQuery), Is.EqualTo("7"));
                break;

            default:
                Action action = () => Build(phrase, connector, insertQuery);
                var exception = Assert.Throws<NotImplementedException>(action);
                Assert.That(exception!.Message, Does.Contain("BuildConflictColumnUpdate"));
                break;
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void HashPhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var isPostgreSql = dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL;
        var isMsSql = dialect.Type == ConnectorBase.SqlServiceType.MSSQL;
        var isOleDb = dialect.Type == ConnectorBase.SqlServiceType.MSACCESS;

        if (isOleDb)
        {
            AssertNotSupported(new MD5("value", ValueObjectType.Literal), "MD5", connector);
            AssertNotSupported(new MD5("value", ValueObjectType.Literal, true), "MD5", connector);
            AssertNotSupported(new SHA1("value", ValueObjectType.Literal), "SHA1", connector);
            AssertNotSupported(new SHA1("value", ValueObjectType.Literal, true), "SHA1", connector);
            return;
        }

        if (isMsSql)
        {
            AssertSql(
                new MD5("value", ValueObjectType.Literal),
                "CONVERT(VARCHAR(32), HASHBYTES('MD5', value), 2)",
                connector);
            AssertSql(
                new MD5("value", ValueObjectType.Literal, true),
                "HASHBYTES('MD5', value)",
                connector);
            AssertSql(
                new SHA1("value", ValueObjectType.Literal),
                "CONVERT(VARCHAR(40), HASHBYTES('SHA1', value), 2)",
                connector);
            AssertSql(
                new SHA1("value", ValueObjectType.Literal, true),
                "HASHBYTES('SHA1', value)",
                connector);
            return;
        }

        AssertSql(
            new MD5("value", ValueObjectType.Literal),
            isPostgreSql ? "md5(value)" : "MD5(value)",
            connector);
        AssertSql(
            new MD5("value", ValueObjectType.Literal, true),
            isPostgreSql ? "decode(md5(value), 'hex')" : "UNHEX(MD5(value))",
            connector);

        if (isPostgreSql)
        {
            AssertNotSupported(new SHA1("value", ValueObjectType.Literal), "SHA1", connector);
            AssertNotSupported(new SHA1("value", ValueObjectType.Literal, true), "SHA1", connector);
            return;
        }

        AssertSql(new SHA1("value", ValueObjectType.Literal), "SHA1(value)", connector);
        AssertSql(new SHA1("value", ValueObjectType.Literal, true), "UNHEX(SHA1(value))", connector);
    }

    [TestCaseSource(nameof(Dialects))]
    public void SpatialPhrases_RenderExpectedSql(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var isMsSql = dialect.Type == ConnectorBase.SqlServiceType.MSSQL;
        var isOleDb = dialect.Type == ConnectorBase.SqlServiceType.MSACCESS;

        if (isOleDb)
        {
            AssertNotImplemented(new ST_X(Literal("point")), "ST_X", connector);
            AssertNotImplemented(new ST_Y(Literal("point")), "ST_Y", connector);
            AssertNotImplemented(
                new GeographySphericalDistanceMath(
                    Literal("from_lat"),
                    Literal("from_lng"),
                    Literal("to_lat"),
                    Literal("to_lng")),
                "spherical distance",
                connector);
            AssertNotImplemented(
                new GeographyContains(Literal("from_point"), Literal("to_point")),
                "ST_Contains",
                connector);
            AssertNotImplemented(
                new GeographyDistance(Literal("from_point"), Literal("to_point")),
                "ST_Distance_Sphere",
                connector);
            AssertNotImplemented(
                new GeographySphericalDistance(Literal("from_point"), Literal("to_point")),
                "spherical distance",
                connector);
            return;
        }

        using (Assert.EnterMultipleScope())
        {
            AssertSql(
                new ST_X(Literal("point")),
                isMsSql ? "point.STX" : "ST_X(point)",
                connector);
            AssertSql(
                new ST_Y(Literal("point")),
                isMsSql ? "point.STY" : "ST_Y(point)",
                connector);
            AssertSql(
                new GeographySphericalDistanceMath(
                    Literal("from_lat"),
                    Literal("from_lng"),
                    Literal("to_lat"),
                    Literal("to_lng")),
                "12742.0 * ASIN(SQRT(POWER(SIN(((from_lat)-(to_lat)) * PI()/360.0), 2) + " +
                "COS(from_lat* PI()/180.0) * COS((to_lat) * PI()/180.0) * " +
                "POWER(SIN((from_lng-to_lng) * PI()/360.0), 2))) * 1000.0",
                connector);
        }

        if (dialect.Type == ConnectorBase.SqlServiceType.MYSQL ||
            dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL)
        {
            var distanceFunction = dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL
                ? "ST_DistanceSphere"
                : "ST_Distance_Sphere";
            AssertSql(
                new GeographyContains(Literal("from_point"), Literal("to_point")),
                "ST_Contains(from_point, to_point)",
                connector);
            AssertSql(
                new GeographyDistance(Literal("from_point"), Literal("to_point")),
                $"{distanceFunction}(from_point, to_point)",
                connector);
            AssertSql(
                new GeographySphericalDistance(Literal("from_point"), Literal("to_point")),
                $"{distanceFunction}(from_point, to_point)",
                connector);
            return;
        }

        if (isMsSql)
        {
            AssertSql(
                new GeographyContains(Literal("from_point"), Literal("to_point")),
                "from_point.STContains(to_point)",
                connector);
        }
        else
        {
            AssertNotImplemented(
                new GeographyContains(Literal("from_point"), Literal("to_point")),
                "ST_Contains",
                connector);
        }

        AssertNotImplemented(
            new GeographyDistance(Literal("from_point"), Literal("to_point")),
            "ST_Distance_Sphere",
            connector);

        var expectedFallback = Build(
            new GeographySphericalDistanceMath(
                ValueWrapper.From(new ST_Y(Literal("from_point"))),
                ValueWrapper.From(new ST_X(Literal("from_point"))),
                ValueWrapper.From(new ST_Y(Literal("to_point"))),
                ValueWrapper.From(new ST_X(Literal("to_point")))),
            connector);
        AssertSql(
            new GeographySphericalDistance(Literal("from_point"), Literal("to_point")),
            expectedFallback,
            connector);
    }

    [TestCaseSource(nameof(Dialects))]
    public void DialectAwareJsonPhrases_RenderOrRejectAsExpected(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);

        AssertPhrase(
            new JsonArray(Literal("one"), Literal("two")),
            dialect.JsonArray,
            "JsonArray is not supported by current DB type",
            connector,
            "JsonArray");
        AssertPhrase(
            new JsonArrayInsert("doc", ValueObjectType.Literal, "$[0]", "value", ValueObjectType.Literal),
            dialect.JsonArrayInsert,
            "JsonArrayInsert is not supported by current DB type",
            connector,
            "JsonArrayInsert");
        AssertPhrase(
            new JsonArrayAppend("doc", ValueObjectType.Literal, "$", "value", ValueObjectType.Literal),
            dialect.JsonArrayAppend,
            "JsonArrayAppend is not supported by current DB type",
            connector,
            "JsonArrayAppend");
        AssertPhrase(
            new JsonInsert("doc", ValueObjectType.Literal, "$.key", "value", ValueObjectType.Literal),
            dialect.JsonInsert,
            "JsonInsert is not supported by current DB type",
            connector,
            "JsonInsert");
        AssertPhrase(
            new JsonLength("doc", ValueObjectType.Literal),
            dialect.JsonLength,
            "JsonLength is not supported by current DB type",
            connector,
            "JsonLength");
        AssertPhrase(
            new JsonObject(new Dictionary<ValueWrapper, ValueWrapper>
            {
                [Literal("key")] = Literal("value"),
            }),
            dialect.JsonObject,
            "JsonObject",
            connector,
            "JsonObject");
        AssertPhrase(
            new JsonSet(Literal("doc"), "$.key", "value", ValueObjectType.Literal),
            dialect.JsonSet,
            "JsonSet",
            connector,
            "JsonSet");
        AssertPhrase(
            new JsonRemove(Literal("doc"), "$.first", "$.second"),
            dialect.JsonRemove,
            "JsonRemove",
            connector,
            "JsonRemove");

        if (dialect.Type == ConnectorBase.SqlServiceType.MYSQL)
        {
            AssertSql(new JsonArrayAggregate(Literal("value")), "JSON_ARRAYAGG(value)", connector);
            AssertSql(
                new JsonContains(Literal("doc"), Literal("candidate"), "$.key"),
                "JSON_CONTAINS(doc, candidate, ('$.key'))",
                connector);
            AssertSql(
                new JsonExtract("doc", ValueObjectType.Literal, "$.key", false),
                "JSON_EXTRACT(doc, '$.key')",
                connector);
            AssertSql(
                new JsonObjectAggregate(Literal("key"), Literal("value")),
                "JSON_OBJECTAGG(key,value)",
                connector);
            AssertSql(
                new JsonValue("doc", ValueObjectType.Literal, "$.key"),
                "JSON_VALUE(doc, '$.key' NULL ON EMPTY NULL ON ERROR)",
                connector);
            AssertSql(
                new MemberOfJsonArray(Literal("value"), Literal("array")),
                "(value MEMBER OF(array))",
                connector);
            return;
        }

        if (dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL)
        {
            AssertSql(new JsonArrayAggregate(Literal("value")), "json_agg(value)", connector);
            AssertSql(
                new JsonContains(Literal("doc"), Literal("candidate"), "$.key"),
                "(jsonb_extract_path((doc)::jsonb, 'key') @> (candidate)::jsonb)",
                connector);
            AssertSql(
                new JsonContains(Literal("doc"), Literal("candidate")),
                "((doc)::jsonb @> (candidate)::jsonb)",
                connector);
            AssertSql(
                new JsonExtract("doc", ValueObjectType.Literal, "$.key", false),
                "(doc #> ARRAY[('key')::text])",
                connector);
            AssertSql(
                new JsonExtract("doc", ValueObjectType.Literal, "$", false),
                "doc",
                connector);
            AssertSql(
                new JsonExtract("doc", ValueObjectType.Literal, "$", true),
                "(doc #>> '{}')",
                connector);
            AssertSql(
                new JsonValue("doc", ValueObjectType.Literal, "$"),
                "(doc #>> '{}')",
                connector);
            AssertSql(
                new JsonObjectAggregate(Literal("key"), Literal("value")),
                "json_object_agg(key,value)",
                connector);
            AssertSql(
                new JsonValue("doc", ValueObjectType.Literal, "$.key"),
                "(doc #>> ARRAY[('key')::text])",
                connector);
            AssertNotImplemented(
                new MemberOfJsonArray(Literal("value"), Literal("array")),
                "MEMBER OF",
                connector);
            return;
        }

        AssertNotImplemented(new JsonArrayAggregate(Literal("value")), "JSON_ARRAYAGG", connector);
        AssertNotImplemented(
            new JsonContains(Literal("doc"), Literal("candidate"), "$.key"),
            "JSON_CONTAINS",
            connector);

        if (dialect.Type == ConnectorBase.SqlServiceType.MSSQL)
        {
            AssertNotSupported(
                new JsonExtract("doc", ValueObjectType.Literal, "$.key", false),
                "raw JSON",
                connector);
            AssertSql(
                new JsonExtract("doc", ValueObjectType.Literal, "$.key", true),
                "JSON_VALUE(doc, (N'$.key'))",
                connector);
            AssertSql(
                new JsonValue("doc", ValueObjectType.Literal, "$.key"),
                "JSON_VALUE(doc, (N'$.key'))",
                connector);
        }
        else
        {
            AssertNotImplemented(
                new JsonExtract("doc", ValueObjectType.Literal, "$.key", false),
                "JSON_EXTRACT",
                connector);
            AssertNotImplemented(
                new JsonValue("doc", ValueObjectType.Literal, "$.key"),
                "JSON_VALUE",
                connector);
        }

        AssertNotImplemented(
            new JsonObjectAggregate(Literal("key"), Literal("value")),
            "JSON_OBJECTAGG",
            connector);
        AssertNotImplemented(
            new MemberOfJsonArray(Literal("value"), Literal("array")),
            "MEMBER OF",
            connector);
    }

    [TestCaseSource(nameof(Dialects))]
    public void JsonValue_NonDefaultActions_AreRenderedOrRejected(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var phrase = new JsonValue(
            "doc", ValueObjectType.Literal, "$.key",
            onEmptyValue: "missing",
            onError: JsonValue.DefaultAction.Error);

        if (dialect.Type == ConnectorBase.SqlServiceType.MYSQL)
        {
            AssertSql(
                phrase,
                "JSON_VALUE(doc, '$.key' DEFAULT 'missing' ON EMPTY ERROR ON ERROR)",
                connector);
            return;
        }

        if (dialect.Type == ConnectorBase.SqlServiceType.MSACCESS)
        {
            AssertNotImplemented(phrase, "JSON_VALUE", connector);
            return;
        }

        AssertNotSupported(phrase, "JSON_VALUE default actions", connector);
    }

    [Test]
    public void MySqlVersionGates_UseSemanticVersionOrdering()
    {
        var olderProviders = new LanguageFactory[]
        {
            new MySqlLanguageFactory(new MySqlMode { Version = "8.0.9-commercial" }),
            new MySql2LanguageFactory(new MySql2Mode { Version = "8.0.9-commercial" }),
        };

        foreach (var provider in olderProviders)
        {
            var connector = new TestConnector(ConnectorBase.SqlServiceType.MYSQL, provider);
            var sql = Build(new JsonValue("doc", ValueObjectType.Literal, "$.key"), connector);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(sql, Does.StartWith("(CASE WHEN JSON_TYPE(JSON_EXTRACT("));
                Assert.That(sql, Does.Not.Contain("JSON_VALUE("));
                Assert.That(provider.GroupBySupportsOrdering, Is.True);
            }
        }

        var currentProviders = new LanguageFactory[]
        {
            new MySqlLanguageFactory(new MySqlMode { Version = "8.0.21-commercial" }),
            new MySql2LanguageFactory(new MySql2Mode { Version = "8.0.21-commercial" }),
        };

        foreach (var provider in currentProviders)
        {
            var connector = new TestConnector(ConnectorBase.SqlServiceType.MYSQL, provider);

            using (Assert.EnterMultipleScope())
            {
                AssertSql(
                    new JsonValue("doc", ValueObjectType.Literal, "$.key"),
                    "JSON_VALUE(doc, '$.key' NULL ON EMPTY NULL ON ERROR)",
                    connector);
                Assert.That(provider.GroupBySupportsOrdering, Is.False);
            }
        }
    }

    [Test]
    public void MySqlJsonPhrases_RejectServerVersionsBeforeFeatureIntroduction()
    {
        foreach (var provider in MySqlProviders("5.7.7"))
        {
            var connector = new TestConnector(ConnectorBase.SqlServiceType.MYSQL, provider);
            var phrases = new IPhrase[]
            {
                new JsonArray(Literal("value")),
                new JsonArrayInsert("doc", ValueObjectType.Literal, "$[0]", "value", ValueObjectType.Literal),
                new JsonInsert("doc", ValueObjectType.Literal, "$.key", "value", ValueObjectType.Literal),
                new JsonLength("doc", ValueObjectType.Literal),
                new JsonObject(new Dictionary<ValueWrapper, ValueWrapper>
                {
                    [Literal("key")] = Literal("value"),
                }),
                new JsonSet(Literal("doc"), "$.key", "value", ValueObjectType.Literal),
                new JsonRemove(Literal("doc"), "$.key"),
                new JsonExtract("doc", ValueObjectType.Literal, "$.key", false),
                new JsonContains(Literal("doc"), Literal("candidate")),
                new JsonValue("doc", ValueObjectType.Literal, "$.key"),
            };

            using (Assert.EnterMultipleScope())
            {
                foreach (var phrase in phrases)
                    AssertNotSupported(phrase, "MySQL 5.7.8", connector);
            }
        }

        foreach (var provider in MySqlProviders("5.7.8"))
        {
            var connector = new TestConnector(ConnectorBase.SqlServiceType.MYSQL, provider);
            AssertNotSupported(
                new JsonArrayAppend(
                    "doc", ValueObjectType.Literal,
                    "$", "value", ValueObjectType.Literal),
                "MySQL 5.7.9",
                connector);
        }

        foreach (var provider in MySqlProviders("5.7.9"))
        {
            var connector = new TestConnector(ConnectorBase.SqlServiceType.MYSQL, provider);
            AssertSql(
                new JsonArrayAppend(
                    "doc", ValueObjectType.Literal,
                    "$", "value", ValueObjectType.Literal),
                "JSON_ARRAY_APPEND(doc, '$', value)",
                connector);
        }

        foreach (var provider in MySqlProviders("5.7.21"))
        {
            var connector = new TestConnector(ConnectorBase.SqlServiceType.MYSQL, provider);

            using (Assert.EnterMultipleScope())
            {
                AssertNotSupported(
                    new JsonArrayAggregate(Literal("value")),
                    "MySQL 5.7.22",
                    connector);
                AssertNotSupported(
                    new JsonObjectAggregate(Literal("key"), Literal("value")),
                    "MySQL 5.7.22",
                    connector);
            }
        }

        foreach (var provider in MySqlProviders("8.0.16"))
        {
            var connector = new TestConnector(ConnectorBase.SqlServiceType.MYSQL, provider);
            AssertNotSupported(
                new MemberOfJsonArray(Literal("value"), Literal("array")),
                "MySQL 8.0.17",
                connector);
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void JsonMutationPhrases_RejectEmptyMutations(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var arrayInsert = new JsonArrayInsert { Document = Literal("doc") };
        var arrayAppend = new JsonArrayAppend
        {
            Document = Literal("doc"),
            Path = "$",
        };
        var insert = new JsonInsert { Document = Literal("doc") };

        using (Assert.EnterMultipleScope())
        {
            AssertInvalidOperation(() => Build(arrayInsert, connector));
            AssertInvalidOperation(() => Build(arrayAppend, connector));
            AssertInvalidOperation(() => Build(insert, connector));
            AssertInvalidOperation(() => Build(new JsonSet(Literal("doc")), connector));
            AssertInvalidOperation(() => Build(new JsonRemove(Literal("doc")), connector));
        }
    }

    [TestCaseSource(nameof(Dialects))]
    public void JsonExtract_DynamicPropertyPathRemainsAnSqlExpression(Dialect dialect)
    {
        var connector = new TestConnector(dialect.Type, dialect.Language);
        var path = new JsonPathExpression(
            JsonPathExpression.Part.Root(),
            new JsonPathExpression.Part
            {
                Value = Literal("property_name"),
                Indexed = false,
            });
        var phrase = new JsonExtract("doc", ValueObjectType.Literal, path, true);

        if (dialect.Type == ConnectorBase.SqlServiceType.MSACCESS)
        {
            AssertNotImplemented(phrase, "JSON_EXTRACT", connector);
            return;
        }

        var sql = Build(phrase, connector);
        var phraseBackedPath = new JsonPathExpression(
            JsonPathExpression.Part.Root(),
            new JsonPathExpression.Part
            {
                Value = ValueWrapper.From(
                    new Concat(true, Literal("property"), Literal("_name"))),
                Indexed = false,
            });
        var phraseBackedSql = Build(
            new JsonExtract(
                "doc", ValueObjectType.Literal,
                phraseBackedPath, true),
            connector);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sql, Does.Contain("property_name"));
            Assert.That(sql, Does.Not.Contain("$.property_name"));
            Assert.That(phraseBackedSql, Does.Contain("property"));
            Assert.That(
                phraseBackedSql,
                Does.Not.Contain(typeof(Concat).FullName));

            if (dialect.Type == ConnectorBase.SqlServiceType.POSTGRESQL)
                Assert.That(sql, Is.EqualTo("(doc #>> ARRAY[property_name::text])"));
            else
                Assert.That(sql, Does.Contain("CONCAT("));
        }
    }

    [Test]
    public void PostgreSqlCollate_QuotesQualifiedIdentifierParts()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.POSTGRESQL,
            new PostgreSQLLanguageFactory(new PostgreSQLMode { Version = "16.0" }));

        AssertSql(
            new Collate(Literal("value"), "tenant.custom\"collation"),
            "(value COLLATE \"tenant\".\"custom\"\"collation\")",
            connector);
    }

    [Test]
    public void Collate_RejectsUnsafeNamesInUnquotedProviders()
    {
        var connectors = new ConnectorBase[]
        {
            new TestConnector(ConnectorBase.SqlServiceType.MYSQL, new LanguageFactory()),
            new TestConnector(
                ConnectorBase.SqlServiceType.MYSQL,
                new MySqlLanguageFactory(new MySqlMode { Version = "8.0.21" })),
            new TestConnector(
                ConnectorBase.SqlServiceType.MYSQL,
                new MySql2LanguageFactory(new MySql2Mode { Version = "8.0.21" })),
            new TestConnector(
                ConnectorBase.SqlServiceType.MSSQL,
                new MsSqlLanguageFactory(new MsSqlVersion { Version = "16.0" })),
        };

        using (Assert.EnterMultipleScope())
        {
            foreach (var connector in connectors)
            {
                Action buildUnsafeCollation = () =>
                {
                    Build(new Collate(
                        Literal("value"),
                        "safe; DROP TABLE items"), connector);
                };

                Assert.Throws<ArgumentException>(buildUnsafeCollation);
            }
        }
    }

    [Test]
    public void PostgreSqlJsonLength_EvaluatesItsOperandOnce()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.POSTGRESQL,
            new PostgreSQLLanguageFactory(new PostgreSQLMode { Version = "16.0" }));
        var sql = Build(
            new JsonLength(new Literal(
                (builder, _, _) => builder.Append("volatile_json()"))),
            connector);

        Assert.That(
            CountOccurrences(sql, "volatile_json()"),
            Is.EqualTo(1));
    }

    [Test]
    public void DialectAwarePhrases_DispatchThroughLanguageProvider()
    {
        var connector = new TestConnector(
            ConnectorBase.SqlServiceType.MYSQL,
            new PostgreSQLLanguageFactory(new PostgreSQLMode { Version = "16.0" }));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                Build(new Concat(false, Literal("left"), Literal("right")), connector),
                Is.EqualTo("left || right"));
            Assert.That(
                Build(new RandWeight("weight", ValueObjectType.Literal), connector),
                Is.EqualTo("RANDOM() * weight"));
            Assert.That(
                Build(new DateTimeAdd(
                    "date_value", ValueObjectType.Literal,
                    DateTimeUnit.Millisecond,
                    2), connector),
                Is.EqualTo("date_value + (2 * INTERVAL '1 millisecond')"));
            Assert.That(
                Build(new JsonArray(Literal("one"), Literal("two")), connector),
                Is.EqualTo("json_build_array(one,two)"));
        }
    }

    private static TestCaseData DialectCase(
        string name,
        ConnectorBase.SqlServiceType type,
        LanguageFactory language)
    {
        var isMySql = type == ConnectorBase.SqlServiceType.MYSQL;
        var isPostgreSql = type == ConnectorBase.SqlServiceType.POSTGRESQL;
        var isMsSql = type == ConnectorBase.SqlServiceType.MSSQL;
        var isOleDb = type == ConnectorBase.SqlServiceType.MSACCESS;

        var dialect = new Dialect
        {
            Type = type,
            Language = language,
            Concat = isPostgreSql
                ? "left || right"
                : isOleDb
                    ? "IIF(IsNull(left) OR IsNull(right), NULL, left & right)"
                    : isMsSql
                        ? "(SELECT CASE WHEN value0 IS NULL OR value1 IS NULL THEN NULL " +
                          "ELSE CONCAT(value0,value1) END FROM (VALUES (left,right)) " +
                          "AS concat_values(value0,value1))"
                        : "CONCAT(left,right)",
            ConcatIgnoringNulls = isPostgreSql
                ? "CONCAT(left,right)"
                : isOleDb
                    ? "Nz(left, '') & Nz(right, '')"
                    : isMsSql
                        ? "CONCAT(left,right)"
                        : "CONCAT(COALESCE(left,''),COALESCE(right,''))",
            EmptyConcat = isMsSql ? "N''" : "''",
            SingleConcat = isPostgreSql
                ? "value"
                : isOleDb
                    ? "IIF(IsNull(value), NULL, value)"
                    : isMsSql
                        ? "(SELECT CASE WHEN value0 IS NULL THEN NULL ELSE CONCAT(N'',value0) END " +
                          "FROM (VALUES (value)) AS concat_values(value0))"
                        : "CONCAT(value)",
            SingleConcatIgnoringNulls = isPostgreSql
                ? "CONCAT(value)"
                : isOleDb
                    ? "Nz(value, '')"
                    : isMsSql
                        ? "CONCAT(N'',value)"
                        : "CONCAT(COALESCE(value,''))",
            RandWeight = isMsSql
                ? "RAND(CAST(NEWID() AS VARBINARY)) * weight"
                : isMySql
                    ? "RAND() * weight"
                    : isOleDb ? "Rnd() * weight" : "RANDOM() * weight",
            DateTimeAdd = isMySql
                ? "TIMESTAMPADD(MICROSECOND,2 * 1000,date_value)"
                : isPostgreSql
                    ? "date_value + (2 * INTERVAL '1 millisecond')"
                    : isOleDb ? null : "DATEADD(millisecond,2,date_value)",
            DateTimeDiff = isMySql
                ? "TIMESTAMPDIFF(MICROSECOND,earlier,later) DIV 1000"
                : isPostgreSql
                    ? "TRUNC(EXTRACT(EPOCH FROM (later - earlier)) * 1000)"
                    : isOleDb ? null : "DATEDIFF(millisecond,earlier,later)",
            JsonArray = isMySql
                ? "JSON_ARRAY(one,two)"
                : isPostgreSql ? "json_build_array(one,two)" : null,
            JsonArrayInsert = isMySql ? "JSON_ARRAY_INSERT(doc, '$[0]', value)" : null,
            JsonArrayAppend = isMySql ? "JSON_ARRAY_APPEND(doc, '$', value)" : null,
            JsonInsert = isMySql ? "JSON_INSERT(doc, '$.key', value)" : null,
            JsonLength = isMySql
                ? "JSON_LENGTH(doc)"
                : isPostgreSql
                    ? "(SELECT CASE WHEN json_value IS NULL THEN NULL " +
                      "WHEN jsonb_typeof(json_value) = 'array' THEN jsonb_array_length(json_value) " +
                      "WHEN jsonb_typeof(json_value) = 'object' THEN " +
                      "(SELECT COUNT(*) FROM jsonb_object_keys(json_value)) ELSE 1 END " +
                      "FROM (SELECT (doc)::jsonb AS json_value OFFSET 0) AS json_input)"
                    : null,
            JsonObject = isMySql
                ? "JSON_OBJECT(key,value)"
                : isPostgreSql ? "json_build_object(key,value)" : null,
            JsonSet = isMySql ? "JSON_SET(doc, '$.key', value)" : null,
            JsonRemove = isMySql ? "JSON_REMOVE(doc, '$.first', '$.second')" : null,
        };

        return new TestCaseData(dialect).SetName($"{{m}}_{name}");
    }

    private static ValueWrapper Literal(string value)
    {
        return ValueWrapper.Literal(value);
    }

    private static int CountOccurrences(string value, string search)
    {
        return value.Split(
            new[] { search },
            StringSplitOptions.None).Length - 1;
    }

    private static IEnumerable<LanguageFactory> MySqlProviders(string version)
    {
        yield return new MySqlLanguageFactory(new MySqlMode { Version = version });
        yield return new MySql2LanguageFactory(new MySql2Mode { Version = version });
    }

    private static IReadOnlyDictionary<DateTimeFormat.FormatOptions, string?> DateTimeFormatsFor(
        ConnectorBase.SqlServiceType type)
    {
        if (type == ConnectorBase.SqlServiceType.MYSQL)
        {
            return new Dictionary<DateTimeFormat.FormatOptions, string?>
            {
                [DateTimeFormat.FormatOptions.IsoDate] = "DATE_FORMAT(value, '%Y-%m-%d')",
                [DateTimeFormat.FormatOptions.IsoDateTime] = "DATE_FORMAT(value, '%Y-%m-%dT%T')",
                [DateTimeFormat.FormatOptions.IsoDateTimeFFF] =
                    "LEFT(DATE_FORMAT(value, '%Y-%m-%dT%T.%f'), 23)",
                [DateTimeFormat.FormatOptions.IsoDateTimeZ] = "DATE_FORMAT(value, '%Y-%m-%dT%TZ')",
                [DateTimeFormat.FormatOptions.IsoDateTimeFFFZ] =
                    "CONCAT(LEFT(DATE_FORMAT(value, '%Y-%m-%dT%T.%f'), 23), 'Z')",
                [DateTimeFormat.FormatOptions.IsoTime] = "DATE_FORMAT(value, '%T')",
                [DateTimeFormat.FormatOptions.IsoTimeFFF] =
                    "LEFT(DATE_FORMAT(value, '%T.%f'), 12)",
                [DateTimeFormat.FormatOptions.IsoYearMonth] = "DATE_FORMAT(value, '%Y-%m')",
            };
        }

        if (type == ConnectorBase.SqlServiceType.POSTGRESQL)
        {
            return new Dictionary<DateTimeFormat.FormatOptions, string?>
            {
                [DateTimeFormat.FormatOptions.IsoDate] = "to_char (value, 'YYYY-MM-DD')",
                [DateTimeFormat.FormatOptions.IsoDateTime] =
                    "to_char (value, 'YYYY-MM-DD\"T\"HH24:MI:SS')",
                [DateTimeFormat.FormatOptions.IsoDateTimeFFF] =
                    "to_char (value, 'YYYY-MM-DD\"T\"HH24:MI:SS.MS')",
                [DateTimeFormat.FormatOptions.IsoDateTimeZ] =
                    "to_char (value::timestamptz at time zone 'UTC', 'YYYY-MM-DD\"T\"HH24:MI:SS\"Z\"')",
                [DateTimeFormat.FormatOptions.IsoDateTimeFFFZ] =
                    "to_char (value::timestamptz at time zone 'UTC', 'YYYY-MM-DD\"T\"HH24:MI:SS.MS\"Z\"')",
                [DateTimeFormat.FormatOptions.IsoTime] = "to_char (value, 'HH24:MI:SS')",
                [DateTimeFormat.FormatOptions.IsoTimeFFF] = "to_char (value, 'HH24:MI:SS.MS')",
                [DateTimeFormat.FormatOptions.IsoYearMonth] = "to_char (value, 'YYYY-MM')",
            };
        }

        if (type == ConnectorBase.SqlServiceType.MSSQL)
        {
            return new Dictionary<DateTimeFormat.FormatOptions, string?>
            {
                [DateTimeFormat.FormatOptions.IsoDate] = "FORMAT(value, 'yyyy-MM-dd')",
                [DateTimeFormat.FormatOptions.IsoDateTime] =
                    "FORMAT(value, 'yyyy-MM-dd\"T\"HH:mm:ss')",
                [DateTimeFormat.FormatOptions.IsoDateTimeFFF] =
                    "FORMAT(value, 'yyyy-MM-dd\"T\"HH:mm:ss.fff')",
                [DateTimeFormat.FormatOptions.IsoDateTimeZ] =
                    "FORMAT(value, 'yyyy-MM-dd\"T\"HH:mm:ss\"Z\"')",
                [DateTimeFormat.FormatOptions.IsoDateTimeFFFZ] =
                    "FORMAT(value, 'yyyy-MM-dd\"T\"HH:mm:ss.fff\"Z\"')",
                [DateTimeFormat.FormatOptions.IsoTime] = "FORMAT(value, 'HH:mm:ss')",
                [DateTimeFormat.FormatOptions.IsoTimeFFF] = "FORMAT(value, 'HH:mm:ss.fff')",
                [DateTimeFormat.FormatOptions.IsoYearMonth] = "FORMAT(value, 'yyyy-MM')",
            };
        }

        return new Dictionary<DateTimeFormat.FormatOptions, string?>
        {
            [DateTimeFormat.FormatOptions.IsoDate] = "FORMAT(value, 'yyyy-mm-dd')",
            [DateTimeFormat.FormatOptions.IsoDateTime] = @"FORMAT(value, 'yyyy-mm-dd\Thh:nn:ss')",
            [DateTimeFormat.FormatOptions.IsoDateTimeFFF] = null,
            [DateTimeFormat.FormatOptions.IsoDateTimeZ] = null,
            [DateTimeFormat.FormatOptions.IsoDateTimeFFFZ] = null,
            [DateTimeFormat.FormatOptions.IsoTime] = "FORMAT(value, 'hh:nn:ss')",
            [DateTimeFormat.FormatOptions.IsoTimeFFF] = null,
            [DateTimeFormat.FormatOptions.IsoYearMonth] = "FORMAT(value, 'yyyy-mm')",
        };
    }

    private static void AssertSql(IPhrase phrase, string expected, ConnectorBase connector)
    {
        Assert.That(Build(phrase, connector), Is.EqualTo(expected), phrase.GetType().Name);
    }

    private static void AssertInvalidOperation(Action action)
    {
        Assert.Throws<InvalidOperationException>(action);
    }

    private static void AssertNotImplemented(
        IPhrase phrase,
        string messageFragment,
        ConnectorBase connector)
    {
        Action action = () => Build(phrase, connector);
        var exception = Assert.Throws<NotImplementedException>(action, phrase.GetType().Name);
        if (exception != null)
            Assert.That(exception.Message, Does.Contain(messageFragment), phrase.GetType().Name);
    }

    private static void AssertNotSupported(
        IPhrase phrase,
        string messageFragment,
        ConnectorBase connector)
    {
        Action action = () => Build(phrase, connector);
        var exception = Assert.Throws<NotSupportedException>(action, phrase.GetType().Name);
        if (exception != null)
            Assert.That(exception.Message, Does.Contain(messageFragment), phrase.GetType().Name);
    }

    private static void AssertPhrase(
        IPhrase phrase,
        string? expected,
        string unsupportedMessage,
        ConnectorBase connector,
        string phraseName)
    {
        if (expected != null)
        {
            Assert.That(Build(phrase, connector), Is.EqualTo(expected), phraseName);
            return;
        }

        Action action = () => Build(phrase, connector);
        var exception = Assert.Throws<NotSupportedException>(action, phraseName);
        if (exception != null)
            Assert.That(exception.Message, Does.Contain(unsupportedMessage), phraseName);
    }

    private static string Build(
        IPhrase phrase,
        ConnectorBase connector,
        Query? relatedQuery = null)
    {
        var builder = new StringBuilder();
        phrase.Build(builder, connector, relatedQuery);
        return builder.ToString();
    }

    public sealed class Dialect
    {
        public ConnectorBase.SqlServiceType Type { get; init; }
        public required LanguageFactory Language { get; init; }
        public required string Concat { get; init; }
        public required string ConcatIgnoringNulls { get; init; }
        public required string EmptyConcat { get; init; }
        public required string SingleConcat { get; init; }
        public required string SingleConcatIgnoringNulls { get; init; }
        public required string RandWeight { get; init; }
        public string? DateTimeAdd { get; init; }
        public string? DateTimeDiff { get; init; }
        public string? JsonArray { get; init; }
        public string? JsonArrayInsert { get; init; }
        public string? JsonArrayAppend { get; init; }
        public string? JsonInsert { get; init; }
        public string? JsonLength { get; init; }
        public string? JsonObject { get; init; }
        public string? JsonSet { get; init; }
        public string? JsonRemove { get; init; }
    }

    private sealed class TestConnector : ConnectorBase
    {
        private readonly SqlServiceType _Type;
        private readonly LanguageFactory _Language;

        public TestConnector(SqlServiceType type, LanguageFactory language)
        {
            _Type = type;
            _Language = language;
        }

        public override SqlServiceType TYPE => _Type;
        public override IConnectorFactory Factory => throw new NotSupportedException();
        public override LanguageFactory Language => _Language;
        public override int ExecuteScript(string querySql) => throw new NotSupportedException();
        public override Task<int> ExecuteScriptAsync(
            string querySql,
            CancellationToken? cancellationToken = null) => throw new NotSupportedException();
        public override object GetLastInsertID() => throw new NotSupportedException();
        public override Task<object> GetLastInsertIdAsync() => throw new NotSupportedException();
        public override bool CheckIfTableExists(string tableName) => throw new NotSupportedException();
        public override Task<bool> CheckIfTableExistsAsync(string tableName) => throw new NotSupportedException();
    }

}
