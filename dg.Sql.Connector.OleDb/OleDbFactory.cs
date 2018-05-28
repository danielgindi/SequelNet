using System;
using System.Collections.Generic;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;

namespace dg.Sql.Connector
{
    public class OleDbFactory : FactoryBase
    {
        internal static OleDbFactory Instance = new OleDbFactory();

        public override DbParameter NewParameter(string name, object value)
        {
            return new OleDbParameter(name, value);
        }

        public override DbParameter NewParameter(string name, DbType type, object value)
        {
            OleDbParameter p = new OleDbParameter(name, value);
            p.DbType = type;
            return p;
        }

        public override DbParameter NewParameter(string name, object value, ParameterDirection parameterDirection)
        {
            OleDbParameter p = new OleDbParameter(name, value);
            p.Direction = parameterDirection;
            return p;
        }

        public override DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection, 
            int size, bool isNullable, 
            byte precision, byte scale, 
            string sourceColumn, DataRowVersion sourceVersion,
            object value)
        {
            OleDbParameter p = new OleDbParameter(name, value);
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
            return new OleDbCommand();
        }

        public override DbCommand NewCommand(string commandText)
        {
            return new OleDbCommand(commandText);
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection)
        {
            return new OleDbCommand(commandText, (OleDbConnection)connection);
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            return new OleDbCommand(commandText, (OleDbConnection)connection, (OleDbTransaction)transaction);
        }

        public override DbDataAdapter NewDataAdapter()
        {
            return new OleDbDataAdapter();
        }

        public override DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            return new OleDbDataAdapter((OleDbCommand)selectCommand);
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            return new OleDbDataAdapter(selectCommandText, (OleDbConnection)connection);
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            return new OleDbDataAdapter(selectCommandText, selectConnString);
        }
    }
}
