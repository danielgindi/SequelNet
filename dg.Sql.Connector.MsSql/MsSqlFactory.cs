using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace dg.Sql.Connector
{
    public class MsSqlFactory : FactoryBase
    {
        public override DbParameter NewParameter(string Name, object Value)
        {
            SqlParameter p = new SqlParameter(Name, Value);
            return p;
        }
        public override DbParameter NewParameter(string Name, DbType Type, object Value)
        {
            SqlParameter p = new SqlParameter(Name, Value);
            p.DbType = Type;
            return p;
        }
        public override DbParameter NewParameter(string Name, object Value, ParameterDirection ParameterDirection)
        {
            SqlParameter p = new SqlParameter(Name, Value);
            p.Direction = ParameterDirection;
            return p;
        }
        public override DbParameter NewParameter(string Name, DbType Type, ParameterDirection ParameterDirection, int Size, bool IsNullable, byte Precision, byte Scale, string SourceColumn, DataRowVersion SourceVersion, object Value)
        {
            SqlParameter p = new SqlParameter(Name, Value);
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
            SqlCommand p = new SqlCommand();
            return p;
        }
        public override DbCommand NewCommand(string CommandText)
        {
            SqlCommand p = new SqlCommand(CommandText);
            return p;
        }
        public override DbCommand NewCommand(string CommandText, DbConnection Connection)
        {
            SqlCommand p = new SqlCommand(CommandText, (SqlConnection)Connection);
            return p;
        }
        public override DbCommand NewCommand(string CommandText, DbConnection Connection, DbTransaction Transaction)
        {
            SqlCommand p = new SqlCommand(CommandText, (SqlConnection)Connection, (SqlTransaction)Transaction);
            return p;
        }

        public override DbDataAdapter NewDataAdapter()
        {
            SqlDataAdapter da = new SqlDataAdapter();
            return da;
        }
        public override DbDataAdapter NewDataAdapter(DbCommand SelectCommand)
        {
            SqlDataAdapter da = new SqlDataAdapter((SqlCommand)SelectCommand);
            return da;
        }
        public override DbDataAdapter NewDataAdapter(string SelectCommandText, DbConnection Connection)
        {
            SqlDataAdapter da = new SqlDataAdapter(SelectCommandText, (SqlConnection)Connection);
            return da;
        }
        public override DbDataAdapter NewDataAdapter(string SelectCommandText, string SelectConnString)
        {
            SqlDataAdapter da = new SqlDataAdapter(SelectCommandText, SelectConnString);
            return da;
        }
    }
}
