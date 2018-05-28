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
        internal static PostgreSQLFactory Instance = new PostgreSQLFactory();

        public override DbParameter NewParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value);
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
            return new NpgsqlCommand();
        }

        public override DbCommand NewCommand(string commandText)
        {
            return new NpgsqlCommand(commandText);
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection)
        {
            return new NpgsqlCommand(commandText, (NpgsqlConnection)connection);
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            return new NpgsqlCommand(commandText, (NpgsqlConnection)connection, (NpgsqlTransaction)transaction);
        }

        public override DbDataAdapter NewDataAdapter()
        {
            return new NpgsqlDataAdapter();
        }

        public override DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            return new NpgsqlDataAdapter((NpgsqlCommand)selectCommand);
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            return new NpgsqlDataAdapter(selectCommandText, (NpgsqlConnection)connection);
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            return new NpgsqlDataAdapter(selectCommandText, selectConnString);
        }
    }
}
