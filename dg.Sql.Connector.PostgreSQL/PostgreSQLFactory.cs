using System;
using System.Collections.Generic;
using System.Web;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Text;

namespace dg.Sql.Connector
{
    public class PostgreSQLFactory : FactoryBase
    {
        public override DbParameter NewParameter(string Name, object Value)
        {
            NpgsqlParameter p = new NpgsqlParameter(Name, Value);
            return p;
        }
        public override DbParameter NewParameter(string Name, DbType Type, object Value)
        {
            NpgsqlParameter p = new NpgsqlParameter(Name, Value);
            p.DbType = Type;
            return p;
        }
        public override DbParameter NewParameter(string Name, object Value, ParameterDirection ParameterDirection)
        {
            NpgsqlParameter p = new NpgsqlParameter(Name, Value);
            p.Direction = ParameterDirection;
            return p;
        }
        public override DbParameter NewParameter(string Name, DbType Type, ParameterDirection ParameterDirection, int Size, bool IsNullable, byte Precision, byte Scale, string SourceColumn, DataRowVersion SourceVersion, object Value)
        {
            NpgsqlParameter p = new NpgsqlParameter(Name, Value);
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
            NpgsqlCommand p = new NpgsqlCommand();
            return p;
        }
        public override DbCommand NewCommand(string CommandText)
        {
            NpgsqlCommand p = new NpgsqlCommand(CommandText);
            return p;
        }
        public override DbCommand NewCommand(string CommandText, DbConnection Connection)
        {
            NpgsqlCommand p = new NpgsqlCommand(CommandText, (NpgsqlConnection)Connection);
            return p;
        }
        public override DbCommand NewCommand(string CommandText, DbConnection Connection, DbTransaction Transaction)
        {
            NpgsqlCommand p = new NpgsqlCommand(CommandText, (NpgsqlConnection)Connection, (NpgsqlTransaction)Transaction);
            return p;
        }

        public override DbDataAdapter NewDataAdapter()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            return da;
        }
        public override DbDataAdapter NewDataAdapter(DbCommand SelectCommand)
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter((NpgsqlCommand)SelectCommand);
            return da;
        }
        public override DbDataAdapter NewDataAdapter(string SelectCommandText, DbConnection Connection)
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(SelectCommandText, (NpgsqlConnection)Connection);
            return da;
        }
        public override DbDataAdapter NewDataAdapter(string SelectCommandText, string SelectConnString)
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(SelectCommandText, SelectConnString);
            return da;
        }
    }
}
