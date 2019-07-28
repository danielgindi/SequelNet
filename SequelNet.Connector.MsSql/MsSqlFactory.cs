using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace SequelNet.Connector
{
    public class MsSqlFactory : IConnectorFactory
    {
        internal static MsSqlFactory Instance = new MsSqlFactory();

        public DbParameter NewParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        public DbParameter NewParameter(string name, DbType type, object value)
        {
            SqlParameter p = new SqlParameter(name, value);
            p.DbType = type;
            return p;
        }

        public DbParameter NewParameter(string name, object value, ParameterDirection ParameterDirection)
        {
            SqlParameter p = new SqlParameter(name, value);
            p.Direction = ParameterDirection;
            return p;
        }

        public DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable, 
            byte precision, byte scale,
            string sourceColumn, DataRowVersion sourceVersion,
            object value)
        {
            SqlParameter p = new SqlParameter(name, value);
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
            return new SqlCommand();
        }

        public DbCommand NewCommand(string commandText)
        {
            return new SqlCommand(commandText);
        }

        public DbCommand NewCommand(string commandText, DbConnection connection)
        {
            return new SqlCommand(commandText, (SqlConnection)connection);
        }

        public DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            return new SqlCommand(commandText, (SqlConnection)connection, (SqlTransaction)transaction);
        }

        public DbDataAdapter NewDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            return new SqlDataAdapter((SqlCommand)selectCommand);
        }

        public DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            return new SqlDataAdapter(selectCommandText, (SqlConnection)connection);
        }

        public DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            return new SqlDataAdapter(selectCommandText, selectConnString);
        }
    }
}
