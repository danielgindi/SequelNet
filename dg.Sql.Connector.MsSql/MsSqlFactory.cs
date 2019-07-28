using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace dg.Sql.Connector
{
    public class MsSqlFactory : FactoryBase
    {
        internal static MsSqlFactory Instance = new MsSqlFactory();

        public override DbParameter NewParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        public override DbParameter NewParameter(string name, DbType type, object value)
        {
            SqlParameter p = new SqlParameter(name, value);
            p.DbType = type;
            return p;
        }

        public override DbParameter NewParameter(string name, object value, ParameterDirection ParameterDirection)
        {
            SqlParameter p = new SqlParameter(name, value);
            p.Direction = ParameterDirection;
            return p;
        }

        public override DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
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

        public override DbCommand NewCommand()
        {
            return new SqlCommand();
        }

        public override DbCommand NewCommand(string commandText)
        {
            return new SqlCommand(commandText);
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection)
        {
            return new SqlCommand(commandText, (SqlConnection)connection);
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            return new SqlCommand(commandText, (SqlConnection)connection, (SqlTransaction)transaction);
        }

        public override DbDataAdapter NewDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public override DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            return new SqlDataAdapter((SqlCommand)selectCommand);
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            return new SqlDataAdapter(selectCommandText, (SqlConnection)connection);
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            return new SqlDataAdapter(selectCommandText, selectConnString);
        }
    }
}
