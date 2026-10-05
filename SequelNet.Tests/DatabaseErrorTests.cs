using System.Reflection;
using SequelNet.Connector;

namespace Tests;

public class DatabaseErrorTests
{
    [TestCase(2601, DatabaseError.UniqueViolation)]
    [TestCase(2627, DatabaseError.UniqueViolation)]
    [TestCase(547, DatabaseError.ConstraintViolation)]
    [TestCase(3728, DatabaseError.UndefinedObject)]
    [TestCase(4924, DatabaseError.UndefinedColumn)]
    [TestCase(2705, DatabaseError.DuplicateColumn)]
    [TestCase(1913, DatabaseError.DuplicateObject)]
    [TestCase(2714, DatabaseError.DuplicateObject)]
    [TestCase(207, DatabaseError.UndefinedColumn)]
    [TestCase(3701, DatabaseError.Unknown)]
    [TestCase(102, DatabaseError.Unknown)]
    public void GetDatabaseError_ClassifiesSqlServerErrors(int number, DatabaseError expected)
    {
        using var connector = new MsSqlConnector("");

        Assert.That(connector.GetDatabaseError(CreateSqlException(number)), Is.EqualTo(expected));
        Assert.That(connector.Connection.State, Is.EqualTo(System.Data.ConnectionState.Closed));
    }

    [Test]
    public void GetDatabaseError_SqlServerInspectsAllErrors()
    {
        using var connector = new MsSqlConnector("");

        Assert.That(connector.GetDatabaseError(CreateSqlException(1750, 2714)),
            Is.EqualTo(DatabaseError.DuplicateObject));
    }

    [TestCase("23505", DatabaseError.UniqueViolation)]
    [TestCase("23503", DatabaseError.ForeignKeyViolation)]
    [TestCase("42701", DatabaseError.DuplicateColumn)]
    [TestCase("42P07", DatabaseError.DuplicateTable)]
    [TestCase("42710", DatabaseError.DuplicateObject)]
    [TestCase("42703", DatabaseError.UndefinedColumn)]
    [TestCase("42704", DatabaseError.UndefinedObject)]
    [TestCase("42P01", DatabaseError.UndefinedTable)]
    [TestCase("42601", DatabaseError.Unknown)]
    public void GetDatabaseError_ClassifiesPostgreSqlErrors(string sqlState, DatabaseError expected)
    {
        using var connector = new PostgreSQLConnector("");
        var exception = new Npgsql.PostgresException("Server error", "ERROR", "ERROR", sqlState);

        Assert.That(connector.GetDatabaseError(exception), Is.EqualTo(expected));
        Assert.That(connector.Connection.State, Is.EqualTo(System.Data.ConnectionState.Closed));
    }

    [Test]
    public void GetDatabaseError_OtherProvidersRejectUnrelatedExceptions([Values] bool usePostgreSql)
    {
        using ConnectorBase connector = usePostgreSql
            ? new PostgreSQLConnector("")
            : new MsSqlConnector("");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(connector.GetDatabaseError(null!), Is.EqualTo(DatabaseError.Unknown));
            Assert.That(connector.GetDatabaseError(CreateException(1062, true)),
                Is.EqualTo(DatabaseError.Unknown));
            Assert.That(connector.GetDatabaseError(new Exception("23505 2627")),
                Is.EqualTo(DatabaseError.Unknown));
        }
    }

    private static Microsoft.Data.SqlClient.SqlException CreateSqlException(params int[] numbers)
    {
        var errorConstructor = typeof(Microsoft.Data.SqlClient.SqlError).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[]
            {
                typeof(int), typeof(byte), typeof(byte), typeof(string),
                typeof(string), typeof(string), typeof(int), typeof(Exception)
            },
            null)!;
        var errors = (Microsoft.Data.SqlClient.SqlErrorCollection)Activator.CreateInstance(
            typeof(Microsoft.Data.SqlClient.SqlErrorCollection), true)!;
        var addError = errors.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)!;

        foreach (var number in numbers)
        {
            var error = errorConstructor.Invoke(new object?[]
            {
                number, (byte)1, (byte)16, "Server", "Server error", "Procedure", 1, null
            });
            addError.Invoke(errors, new[] { error });
        }

        var createException = typeof(Microsoft.Data.SqlClient.SqlException).GetMethod(
            "CreateException",
            BindingFlags.Static | BindingFlags.NonPublic,
            null,
            new[] { typeof(Microsoft.Data.SqlClient.SqlErrorCollection), typeof(string) },
            null)!;

        return (Microsoft.Data.SqlClient.SqlException)createException.Invoke(
            null, new object[] { errors, "16.0" })!;
    }

    [TestCase(1062, DatabaseError.UniqueViolation, false)]
    [TestCase(1062, DatabaseError.UniqueViolation, true)]
    [TestCase(1452, DatabaseError.ForeignKeyViolation, false)]
    [TestCase(1452, DatabaseError.ForeignKeyViolation, true)]
    [TestCase(1091, DatabaseError.UndefinedObject, false)]
    [TestCase(1091, DatabaseError.UndefinedObject, true)]
    [TestCase(1060, DatabaseError.DuplicateColumn, false)]
    [TestCase(1060, DatabaseError.DuplicateColumn, true)]
    [TestCase(1050, DatabaseError.DuplicateTable, false)]
    [TestCase(1050, DatabaseError.DuplicateTable, true)]
    [TestCase(1054, DatabaseError.UndefinedColumn, false)]
    [TestCase(1054, DatabaseError.UndefinedColumn, true)]
    [TestCase(1061, DatabaseError.DuplicateIndex, false)]
    [TestCase(1061, DatabaseError.DuplicateIndex, true)]
    [TestCase(1064, DatabaseError.Unknown, false)]
    [TestCase(1064, DatabaseError.Unknown, true)]
    public void GetDatabaseError_ClassifiesProviderExceptions(
        int number, DatabaseError expected, bool useMySqlConnector)
    {
        using var connector = CreateConnector(useMySqlConnector);
        var exception = CreateException(number, useMySqlConnector);

        Assert.That(connector.GetDatabaseError(exception), Is.EqualTo(expected));
        Assert.That(connector.Connection.State, Is.EqualTo(System.Data.ConnectionState.Closed));
    }

    [Test]
    public void GetDatabaseError_UnrecognizedExceptionsReturnUnknown([Values] bool useMySqlConnector)
    {
        using var connector = CreateConnector(useMySqlConnector);
        var providerException = CreateException(1062, useMySqlConnector);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(connector.GetDatabaseError(null!), Is.EqualTo(DatabaseError.Unknown));
            Assert.That(connector.GetDatabaseError(new InvalidOperationException("1062")),
                Is.EqualTo(DatabaseError.Unknown));
            Assert.That(connector.GetDatabaseError(CreateException(1062, !useMySqlConnector)),
                Is.EqualTo(DatabaseError.Unknown));
            Assert.That(connector.GetDatabaseError(new Exception("Wrapped", providerException)),
                Is.EqualTo(DatabaseError.Unknown));
        }
    }

    [Test]
    public void GetDatabaseError_CanBeUsedInCatchFilters([Values] bool useMySqlConnector)
    {
        using var connector = CreateConnector(useMySqlConnector);
        var exception = CreateException(1062, useMySqlConnector);
        Exception? caught = null;

        try
        {
            throw exception;
        }
        catch (Exception ex) when (connector.GetDatabaseError(ex) == DatabaseError.UniqueViolation)
        {
            caught = ex;
        }

        Assert.That(caught, Is.SameAs(exception));
    }

    private static ConnectorBase CreateConnector(bool useMySqlConnector)
    {
        return useMySqlConnector
            ? new MySql2Connector("")
            : new SequelNet.Connector.MySqlConnector("");
    }

    private static Exception CreateException(int number, bool useMySqlConnector)
    {
        // Both drivers keep their server-error constructors internal.
        // Construct real provider exceptions without requiring a database server.
        if (useMySqlConnector)
        {
            var constructor = typeof(global::MySqlConnector.MySqlException).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(global::MySqlConnector.MySqlErrorCode), typeof(string) },
                null)!;

            return (Exception)constructor.Invoke(new object[]
            {
                (global::MySqlConnector.MySqlErrorCode)number, "Server error"
            });
        }

        var dataConstructor = typeof(MySql.Data.MySqlClient.MySqlException).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(string), typeof(int) },
            null)!;

        return (Exception)dataConstructor.Invoke(new object[] { "Server error", number });
    }
}
