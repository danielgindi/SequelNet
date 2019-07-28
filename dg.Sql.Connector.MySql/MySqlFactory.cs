using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace dg.Sql.Connector
{
    public class MySqlFactory : FactoryBase
    {
        internal static MySqlFactory Instance = new MySqlFactory();

        public override DbParameter NewParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }

        public override DbParameter NewParameter(string name, DbType type, object value)
        {
            MySqlParameter p = new MySqlParameter(name, value);
            p.DbType = type;
            return p;
        }

        public override DbParameter NewParameter(string name, object value, ParameterDirection parameterDirection)
        {
            MySqlParameter p = new MySqlParameter(name, value);
            p.Direction = parameterDirection;
            return p;
        }

        public override DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable,
            byte precision, byte scale,
            string sourceColumn, DataRowVersion sourceVersion,
            object value)
        {
            MySqlParameter p = new MySqlParameter(name, value);
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
            return new MySqlCommand();
        }

        public override DbCommand NewCommand(string commandText)
        {
            return new MySqlCommand(commandText);
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection)
        {
            return new MySqlCommand(commandText, (MySqlConnection)connection);
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            return new MySqlCommand(commandText, (MySqlConnection)connection, (MySqlTransaction)transaction);
        }

        public override DbDataAdapter NewDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        public override DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            return new MySqlDataAdapter((MySqlCommand)selectCommand);
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            return new MySqlDataAdapter(selectCommandText, (MySqlConnection)connection);
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            return new MySqlDataAdapter(selectCommandText, selectConnString);
        }
    }
}
