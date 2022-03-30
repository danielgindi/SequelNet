using System.Data;
using System.Data.Common;

namespace SequelNet.Connector
{
    public interface IConnectorFactory
    {
        string ConnectionString { get; }

        ConnectorBase Connector();

        DbParameter NewParameter(string name, object value);

        DbParameter NewParameter(string name, DbType type, object value);

        DbParameter NewParameter(string name, object value, ParameterDirection parameterDirection);

        DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable,
            byte precision, byte scale,
            string sourceColumn, DataRowVersion sourceVersion,
            object value);

        DbCommand NewCommand();

        DbCommand NewCommand(string commandText);

        DbCommand NewCommand(string commandText, DbConnection connection);

        DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction);

        DbDataAdapter NewDataAdapter();

        DbDataAdapter NewDataAdapter(DbCommand selectCommand);

        DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection);

        DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString);
    }
}
