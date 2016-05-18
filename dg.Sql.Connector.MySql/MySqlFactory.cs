using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Text;

namespace dg.Sql.Connector
{
    public class MySqlFactory : FactoryBase
    {
        public override DbParameter NewParameter(string name, object value)
        {
            MySqlParameter p = new MySqlParameter(name, value);
            return p;
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
            MySqlCommand p = new MySqlCommand();
            return p;
        }

        public override DbCommand NewCommand(string commandText)
        {
            MySqlCommand p = new MySqlCommand(commandText);
            return p;
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection)
        {
            MySqlCommand p = new MySqlCommand(commandText, (MySqlConnection)connection);
            return p;
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            MySqlCommand p = new MySqlCommand(commandText, (MySqlConnection)connection, (MySqlTransaction)transaction);
            return p;
        }

        public override DbDataAdapter NewDataAdapter()
        {
            MySqlDataAdapter da = new MySqlDataAdapter();
            return da;
        }

        public override DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            MySqlDataAdapter da = new MySqlDataAdapter((MySqlCommand)selectCommand);
            return da;
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            MySqlDataAdapter da = new MySqlDataAdapter(selectCommandText, (MySqlConnection)connection);
            return da;
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            MySqlDataAdapter da = new MySqlDataAdapter(selectCommandText, selectConnString);
            return da;
        }
    }
}
