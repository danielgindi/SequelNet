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
        public override DbParameter NewParameter(string Name, object Value)
        {
            OleDbParameter p = new OleDbParameter(Name, Value);
            return p;
        }
        public override DbParameter NewParameter(string Name, DbType Type, object Value)
        {
            OleDbParameter p = new OleDbParameter(Name, Value);
            p.DbType = Type;
            return p;
        }
        public override DbParameter NewParameter(string Name, object Value, ParameterDirection ParameterDirection)
        {
            OleDbParameter p = new OleDbParameter(Name, Value);
            p.Direction = ParameterDirection;
            return p;
        }
        public override DbParameter NewParameter(string Name, DbType Type, ParameterDirection ParameterDirection, int Size, bool IsNullable, byte Precision, byte Scale, string SourceColumn, DataRowVersion SourceVersion, object Value)
        {
            OleDbParameter p = new OleDbParameter(Name, Value);
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
            OleDbCommand p = new OleDbCommand();
            return p;
        }
        public override DbCommand NewCommand(string CommandText)
        {
            OleDbCommand p = new OleDbCommand(CommandText);
            return p;
        }
        public override DbCommand NewCommand(string CommandText, DbConnection Connection)
        {
            OleDbCommand p = new OleDbCommand(CommandText, (OleDbConnection)Connection);
            return p;
        }
        public override DbCommand NewCommand(string CommandText, DbConnection Connection, DbTransaction Transaction)
        {
            OleDbCommand p = new OleDbCommand(CommandText, (OleDbConnection)Connection, (OleDbTransaction)Transaction);
            return p;
        }

        public override DbDataAdapter NewDataAdapter()
        {
            OleDbDataAdapter da = new OleDbDataAdapter();
            return da;
        }
        public override DbDataAdapter NewDataAdapter(DbCommand SelectCommand)
        {
            OleDbDataAdapter da = new OleDbDataAdapter((OleDbCommand)SelectCommand);
            return da;
        }
        public override DbDataAdapter NewDataAdapter(string SelectCommandText, DbConnection Connection)
        {
            OleDbDataAdapter da = new OleDbDataAdapter(SelectCommandText, (OleDbConnection)Connection);
            return da;
        }
        public override DbDataAdapter NewDataAdapter(string SelectCommandText, string SelectConnString)
        {
            OleDbDataAdapter da = new OleDbDataAdapter(SelectCommandText, SelectConnString);
            return da;
        }
    }
}
