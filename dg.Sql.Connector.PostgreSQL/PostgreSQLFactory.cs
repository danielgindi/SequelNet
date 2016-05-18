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
        public override DbParameter NewParameter(string name, object value)
        {
            NpgsqlParameter p = new NpgsqlParameter(name, value);
            return p;
        }

        public override DbParameter NewParameter(string name, DbType type, object value)
        {
            NpgsqlParameter p = new NpgsqlParameter(name, value);
            p.DbType = type;
            return p;
        }

        public override DbParameter NewParameter(string name, object value, ParameterDirection parameterDirection)
        {
            NpgsqlParameter p = new NpgsqlParameter(name, value);
            p.Direction = parameterDirection;
            return p;
        }

        public override DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable,
            byte precision, byte scale,
            string sourceColumn, DataRowVersion sourceVersion,
            object value)
        {
            NpgsqlParameter p = new NpgsqlParameter(name, value);
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
            NpgsqlCommand p = new NpgsqlCommand();
            return p;
        }

        public override DbCommand NewCommand(string commandText)
        {
            NpgsqlCommand p = new NpgsqlCommand(commandText);
            return p;
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection)
        {
            NpgsqlCommand p = new NpgsqlCommand(commandText, (NpgsqlConnection)connection);
            return p;
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            NpgsqlCommand p = new NpgsqlCommand(commandText, (NpgsqlConnection)connection, (NpgsqlTransaction)transaction);
            return p;
        }

        public override DbDataAdapter NewDataAdapter()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            return da;
        }

        public override DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter((NpgsqlCommand)selectCommand);
            return da;
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(selectCommandText, (NpgsqlConnection)connection);
            return da;
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(selectCommandText, selectConnString);
            return da;
        }
    }
}
