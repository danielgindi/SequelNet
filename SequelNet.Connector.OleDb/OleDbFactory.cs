using System.Data.OleDb;
using System.Data;
using System.Data.Common;

namespace SequelNet.Connector;

public class OleDbFactory : IConnectorFactory
{
    internal static OleDbFactory Shared = new OleDbFactory(null);

    public string ConnectionString { get; set; }

    public OleDbFactory(string connectionString)
    {
        this.ConnectionString = connectionString;
    }

    public DbParameter NewParameter(string name, object value)
    {
        return new OleDbParameter(name, value);
    }

    public ConnectorBase Connector()
    {
        return new OleDbConnector(this);
    }

    public DbParameter NewParameter(string name, DbType type, object value)
    {
        OleDbParameter p = new OleDbParameter(name, value);
        p.DbType = type;
        return p;
    }

    public DbParameter NewParameter(string name, object value, ParameterDirection parameterDirection)
    {
        OleDbParameter p = new OleDbParameter(name, value);
        p.Direction = parameterDirection;
        return p;
    }

    public DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection, 
        int size, bool isNullable, 
        byte precision, byte scale, 
        string sourceColumn, DataRowVersion sourceVersion,
        object value)
    {
        OleDbParameter p = new OleDbParameter(name, value);
        p.DbType = type;
        p.Direction = parameterDirection;
        p.Size = size;
        p.IsNullable = isNullable;
        p.Precision = precision;
        p.Scale = scale;
        p.SourceColumn = sourceColumn;
        p.SourceVersion = sourceVersion;
        return p;
    }

    public DbCommand NewCommand()
    {
        return new OleDbCommand();
    }

    public DbCommand NewCommand(string commandText)
    {
        return new OleDbCommand(commandText);
    }

    public DbCommand NewCommand(string commandText, DbConnection connection)
    {
        return new OleDbCommand(commandText, (OleDbConnection)connection);
    }

    public DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
    {
        return new OleDbCommand(commandText, (OleDbConnection)connection, (OleDbTransaction)transaction);
    }

    public DbDataAdapter NewDataAdapter()
    {
        return new OleDbDataAdapter();
    }

    public DbDataAdapter NewDataAdapter(DbCommand selectCommand)
    {
        return new OleDbDataAdapter((OleDbCommand)selectCommand);
    }

    public DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
    {
        return new OleDbDataAdapter(selectCommandText, (OleDbConnection)connection);
    }

    public DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
    {
        return new OleDbDataAdapter(selectCommandText, selectConnString);
    }
}
