using System.Data;
using System.Data.Common;
using SequelNet.Connector;

namespace Tests;

public class MySqlVersionDetectionTests
{
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("unknown", false)]
    [TestCase("8", false)]
    [TestCase("8.0.36-commercial", true)]
    [TestCase("5.7.21", true)]
    public void Mode_ReportsWhetherVersionIsKnown(string? version, bool expected)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(new MySqlMode { Version = version }.IsVersionKnown, Is.EqualTo(expected));
            Assert.That(new MySql2Mode { Version = version }.IsVersionKnown, Is.EqualTo(expected));
        }
    }

    [TestCase(false, null)]
    [TestCase(true, null)]
    [TestCase(false, "unknown")]
    [TestCase(true, "unknown")]
    [TestCase(false, "8")]
    [TestCase(true, "8")]
    public void Language_RetriesUnknownVersionsAndRefreshesFeatureChecks(bool useMySql2, string? initialVersion)
    {
        var attempts = 0;
        using var connector = CreateConnector(useMySql2, Guid.NewGuid().ToString(), query =>
        {
            if (query == "SELECT @@SQL_MODE")
                return "ANSI_QUOTES";

            attempts++;
            return attempts == 1 ? initialVersion! : "5.7.21";
        });

        var unknownLanguage = connector.Language;
        Assert.That(attempts, Is.EqualTo(1));

        var knownLanguage = connector.Language;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(attempts, Is.EqualTo(2));
            Assert.That(knownLanguage, Is.Not.SameAs(unknownLanguage));
            Assert.That(unknownLanguage.GroupBySupportsOrdering, Is.False);
            Assert.That(knownLanguage.GroupBySupportsOrdering, Is.True);
            Assert.That(connector.GetVersion(), Is.EqualTo("5.7.21"));
            Assert.That(connector.Language, Is.SameAs(knownLanguage));
            Assert.That(attempts, Is.EqualTo(2));
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void GetVersion_RetriesFailedDiscovery(bool useMySql2)
    {
        var attempts = 0;
        using var connector = CreateConnector(useMySql2, Guid.NewGuid().ToString(), query =>
        {
            if (query == "SELECT @@SQL_MODE")
                return "";

            if (++attempts == 1)
                throw new InvalidOperationException("Version discovery temporarily unavailable");

            return "8.0.36-commercial";
        });

        Assert.That(connector.GetVersion(), Is.Null);
        Assert.That(connector.GetVersion(), Is.EqualTo("8.0.36-commercial"));
        Assert.That(attempts, Is.EqualTo(2));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void UnknownVersion_DoesNotPoisonSharedCache(bool useMySql2)
    {
        var connectionString = Guid.NewGuid().ToString();
        var unknownAttempts = 0;
        using var unknownConnector = CreateConnector(useMySql2, connectionString, query =>
        {
            if (query == "SELECT @@SQL_MODE")
                return "";

            unknownAttempts++;
            return "unknown";
        });
        var unknownLanguage = unknownConnector.Language;

        var knownAttempts = 0;
        using var knownConnector = CreateConnector(useMySql2, connectionString, query =>
        {
            if (query == "SELECT @@SQL_MODE")
                return "";

            knownAttempts++;
            return "8.0.36";
        });
        var knownLanguage = knownConnector.Language;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(knownAttempts, Is.EqualTo(1));
            Assert.That(knownConnector.GetVersion(), Is.EqualTo("8.0.36"));
            Assert.That(unknownConnector.GetVersion(), Is.EqualTo("8.0.36"));
            Assert.That(unknownAttempts, Is.EqualTo(1));
            Assert.That(unknownConnector.Language, Is.SameAs(knownLanguage));
            Assert.That(knownLanguage, Is.Not.SameAs(unknownLanguage));
        }
    }

    private static ConnectorBase CreateConnector(bool useMySql2, string connectionString, Func<string, object> executeScalar)
    {
        return useMySql2
            ? new TestMySql2Connector(connectionString, executeScalar)
            : new TestMySqlConnector(connectionString, executeScalar);
    }

    private sealed class TestMySqlConnector : SequelNet.Connector.MySqlConnector
    {
        private readonly Func<string, object> _ExecuteScalar;

        public TestMySqlConnector(string connectionString, Func<string, object> executeScalar) : base("")
        {
            Connection.Dispose();
            Connection = new TestConnection(connectionString);
            _ExecuteScalar = executeScalar;
        }

        public override object ExecuteScalar(string querySql) => _ExecuteScalar(querySql);
    }

    private sealed class TestMySql2Connector : MySql2Connector
    {
        private readonly Func<string, object> _ExecuteScalar;

        public TestMySql2Connector(string connectionString, Func<string, object> executeScalar) : base("")
        {
            Connection.Dispose();
            Connection = new TestConnection(connectionString);
            _ExecuteScalar = executeScalar;
        }

        public override object ExecuteScalar(string querySql) => _ExecuteScalar(querySql);
    }

    private sealed class TestConnection(string connectionString) : DbConnection
    {
        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string ConnectionString { get; set; } = connectionString;
        public override string Database => "test";
        public override string DataSource => "test";
        public override string ServerVersion => throw new NotSupportedException();
        public override ConnectionState State => ConnectionState.Open;
        public override void ChangeDatabase(string databaseName) => throw new NotSupportedException();
        public override void Close() { }
        public override void Open() { }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();
    }
}
