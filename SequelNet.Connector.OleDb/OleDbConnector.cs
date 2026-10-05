using System;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]

namespace SequelNet.Connector;

public class OleDbConnector : ConnectorBase
{
    public override DatabaseError GetDatabaseError(Exception exception)
    {
        if (exception is not OleDbException oleDbException)
            return DatabaseError.Unknown;

        foreach (OleDbError error in oleDbException.Errors)
        {
            // Jet exposes Access error numbers through SQLState, not NativeError.
            // Restrict the mapping to Jet/ACE error records, including for linked tables.
            if (!string.Equals(error.Source, "Microsoft JET Database Engine", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(error.Source, "Microsoft Access Database Engine", StringComparison.OrdinalIgnoreCase))
                continue;

            var category = error.SQLState switch
            {
                "3022" => DatabaseError.UniqueViolation,
                "3200" or "3201" => DatabaseError.ForeignKeyViolation,
                "3191" or "3380" => DatabaseError.DuplicateColumn,
                "3010" => DatabaseError.DuplicateTable,
                "3284" or "3375" => DatabaseError.DuplicateIndex,
                "3012" or "3283" => DatabaseError.DuplicateObject,
                "3372" => DatabaseError.UndefinedObject,
                "3381" => DatabaseError.UndefinedColumn,
                "3376" => DatabaseError.UndefinedTable,
                _ => DatabaseError.Unknown
            };

            if (category != DatabaseError.Unknown)
                return category;
        }

        return DatabaseError.Unknown;
    }

    #region Instancing

    private OleDbFactory _Factory;

    public override SqlServiceType TYPE
    {
        get { return SqlServiceType.MSACCESS; }
    }

    public static OleDbConnection CreateSqlConnection(string connectionString)
    {
        return new OleDbConnection(connectionString);
    }

    public OleDbConnector(OleDbFactory factory)
    {
        _Factory = factory;
        Connection = CreateSqlConnection(_Factory.ConnectionString);
    }

    public OleDbConnector(string connectionString)
    {
        _Factory = OleDbFactory.Shared;
        Connection = CreateSqlConnection(connectionString);
    }

    ~OleDbConnector()
    {
        Dispose(false);
    }

    public override IConnectorFactory Factory => _Factory;

    private static OleDbLanguageFactory _LanguageFactory = new OleDbLanguageFactory();

    public override LanguageFactory Language => _LanguageFactory;

    #endregion

    #region Executing

    public override int ExecuteScript(string querySql)
    {
        return ExecuteNonQuery(querySql);
    }

    public override Task<int> ExecuteScriptAsync(string querySql, CancellationToken? cancellationToken = null)
    {
        return ExecuteNonQueryAsync(querySql, cancellationToken);
    }

    #endregion

    #region Utilities

    public override bool SupportsSelectPaging()
    {
        return false;
    }

    public OleDbConnection GetUnderlyingConnection()
    {
        return (OleDbConnection)Connection;
    }

    public override object GetLastInsertID()
    {
        return ExecuteScalar("SELECT @@identity AS id");
    }

    public override Task<object> GetLastInsertIdAsync()
    {
        return ExecuteScalarAsync(@"SELECT @@identity AS id");
    }

    public override bool CheckIfTableExists(string tableName)
    {
        return ExecuteScalar($"SELECT name FROM MSysObjects WHERE name like {Language.PrepareValue(tableName)}") != null;
    }

    public override async Task<bool> CheckIfTableExistsAsync(string tableName)
    {
        return await ExecuteScalarAsync($"SELECT name FROM MSysObjects WHERE name like {Language.PrepareValue(tableName)}") != null;
    }

    #endregion
}
