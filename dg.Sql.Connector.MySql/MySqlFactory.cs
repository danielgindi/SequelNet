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
        public override DbParameter NewParameter(string Name, object Value)
        {
            MySqlParameter p = new MySqlParameter(Name, Value);
            return p;
        }
        public override DbParameter NewParameter(string Name, DbType Type, object Value)
        {
            MySqlParameter p = new MySqlParameter(Name, Value);
            p.DbType = Type;
            return p;
        }
        public override DbParameter NewParameter(string Name, object Value, ParameterDirection ParameterDirection)
        {
            MySqlParameter p = new MySqlParameter(Name, Value);
            p.Direction = ParameterDirection;
            return p;
        }
        public override DbParameter NewParameter(string Name, DbType Type, ParameterDirection ParameterDirection, int Size, bool IsNullable, byte Precision, byte Scale, string SourceColumn, DataRowVersion SourceVersion, object Value)
        {
            MySqlParameter p = new MySqlParameter(Name, Value);
            p.DbType = Type;
            p.Direction = ParameterDirection;
            p.Size = Size;
            p.IsNullable = IsNullable;
            p.Precision = Precision;
            p.Scale = Scale;
            p.SourceColumn = SourceColumn;
            p.SourceVersion = SourceVersion;
            return p;
        }

        public override DbCommand NewCommand()
        {
            MySqlCommand p = new MySqlCommand();
            return p;
        }
        public override DbCommand NewCommand(string CommandText)
        {
            MySqlCommand p = new MySqlCommand(CommandText);
            return p;
        }
        public override DbCommand NewCommand(string CommandText, DbConnection Connection)
        {
            MySqlCommand p = new MySqlCommand(CommandText, (MySqlConnection)Connection);
            return p;
        }
        public override DbCommand NewCommand(string CommandText, DbConnection Connection, DbTransaction Transaction)
        {
            MySqlCommand p = new MySqlCommand(CommandText, (MySqlConnection)Connection, (MySqlTransaction)Transaction);
            return p;
        }

        public override DbDataAdapter NewDataAdapter()
        {
            MySqlDataAdapter da = new MySqlDataAdapter();
            return da;
        }
        public override DbDataAdapter NewDataAdapter(DbCommand SelectCommand)
        {
            MySqlDataAdapter da = new MySqlDataAdapter((MySqlCommand)SelectCommand);
            return da;
        }
        public override DbDataAdapter NewDataAdapter(string SelectCommandText, DbConnection Connection)
        {
            MySqlDataAdapter da = new MySqlDataAdapter(SelectCommandText, (MySqlConnection)Connection);
            return da;
        }
        public override DbDataAdapter NewDataAdapter(string SelectCommandText, string SelectConnString)
        {
            MySqlDataAdapter da = new MySqlDataAdapter(SelectCommandText, SelectConnString);
            return da;
        }
    }
}
