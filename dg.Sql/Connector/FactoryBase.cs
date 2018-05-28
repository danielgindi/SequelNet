using System.Data;
using System.Data.Common;

namespace dg.Sql.Connector
{
    public abstract class FactoryBase
    {
        public abstract DbParameter NewParameter(string name, object value);

        public abstract DbParameter NewParameter(string name, DbType type, object value);

        public abstract DbParameter NewParameter(string name, object value, ParameterDirection parameterDirection);

        public abstract DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable,
            byte precision, byte scale,
            string sourceColumn, DataRowVersion sourceVersion,
            object value);

        public abstract DbCommand NewCommand();

        public abstract DbCommand NewCommand(string commandText);

        public abstract DbCommand NewCommand(string commandText, DbConnection connection);

        public abstract DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction);

        public abstract DbDataAdapter NewDataAdapter();

        public abstract DbDataAdapter NewDataAdapter(DbCommand selectCommand);

        public abstract DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection);

        public abstract DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString);
    }
}
