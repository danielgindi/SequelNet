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
        public override DbParameter NewParameter(string name, object value)
        {
            OleDbParameter p = new OleDbParameter(name, value);
            return p;
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
            OleDbCommand p = new OleDbCommand();
            return p;
        }

        public override DbCommand NewCommand(string commandText)
        {
            OleDbCommand p = new OleDbCommand(commandText);
            return p;
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection)
        {
            OleDbCommand p = new OleDbCommand(commandText, (OleDbConnection)connection);
            return p;
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            OleDbCommand p = new OleDbCommand(commandText, (OleDbConnection)connection, (OleDbTransaction)transaction);
            return p;
        }

        public override DbDataAdapter NewDataAdapter()
        {
            OleDbDataAdapter da = new OleDbDataAdapter();
            return da;
        }

        public override DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            OleDbDataAdapter da = new OleDbDataAdapter((OleDbCommand)selectCommand);
            return da;
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            OleDbDataAdapter da = new OleDbDataAdapter(selectCommandText, (OleDbConnection)connection);
            return da;
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            OleDbDataAdapter da = new OleDbDataAdapter(selectCommandText, selectConnString);
            return da;
        }
    }
}
