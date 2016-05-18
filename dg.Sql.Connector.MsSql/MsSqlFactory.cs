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
        public override DbParameter NewParameter(string name, object value)
        {
            SqlParameter p = new SqlParameter(name, value);
            return p;
        }

        public override DbParameter NewParameter(string name, DbType type, object value)
        {
            SqlParameter p = new SqlParameter(name, value);
            p.DbType = type;
            return p;
        }

        public override DbParameter NewParameter(string name, object value, ParameterDirection ParameterDirection)
        {
            SqlParameter p = new SqlParameter(name, value);
            p.Direction = ParameterDirection;
            return p;
        }

        public override DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable, 
            byte precision, byte scale,
            string sourceColumn, DataRowVersion sourceVersion,
            object value)
        {
            SqlParameter p = new SqlParameter(name, value);
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
            SqlCommand p = new SqlCommand();
            return p;
        }

        public override DbCommand NewCommand(string commandText)
        {
            SqlCommand p = new SqlCommand(commandText);
            return p;
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection)
        {
            SqlCommand p = new SqlCommand(commandText, (SqlConnection)connection);
            return p;
        }

        public override DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            SqlCommand p = new SqlCommand(commandText, (SqlConnection)connection, (SqlTransaction)transaction);
            return p;
        }

        public override DbDataAdapter NewDataAdapter()
        {
            SqlDataAdapter da = new SqlDataAdapter();
            return da;
        }

        public override DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            SqlDataAdapter da = new SqlDataAdapter((SqlCommand)selectCommand);
            return da;
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            SqlDataAdapter da = new SqlDataAdapter(selectCommandText, (SqlConnection)connection);
            return da;
        }

        public override DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            SqlDataAdapter da = new SqlDataAdapter(selectCommandText, selectConnString);
            return da;
        }
    }
}
