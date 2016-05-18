using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;

namespace dg.Sql.Connector
{
    public abstract class FactoryBase
    {
        static private Type s_FactoryType = null;
        static private FactoryBase s_Instance = null;

        public static FactoryBase Factory()
        {
            if (s_Instance == null)
            {
                if (s_FactoryType == null)
                {
                    s_FactoryType = FindFactoryType();
                }
                s_Instance = System.Activator.CreateInstance(s_FactoryType) as FactoryBase;
            }
            return s_Instance;
        }
        static private Type FindFactoryType()
        {
            Type type = null;
            try
            {
                string connectorType = ConfigurationManager.AppSettings[@"dg.Sql.Connector"];
                if (!string.IsNullOrEmpty(connectorType))
                {
                    Assembly asm = Assembly.Load(@"dg.Sql.Connector." + connectorType);
                    type = asm.GetType(@"dg.Sql.Connector." + connectorType + @"Factory");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(@"dg.Sql.Connector.Base.FindMgrType error: {0}", ex));
            }
            if (type == null) System.Web.Compilation.BuildManager.GetType(@"dg.Sql.Connector.MySqlFactory", false);
            return type;
        }

        public abstract DbParameter NewParameter(string name, object value);
        public abstract DbParameter NewParameter(string name, DbType type, object value);
        public abstract DbParameter NewParameter(string name, object value, ParameterDirection parameterDirection);
        public abstract DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable,
            byte precision, byte scale,
            string sourceColumn, DataRowVersion sourceVersion,
            object value);

        public abstract DbCommand NewCommand();
        public abstract DbCommand NewCommand(string commandText);
        public abstract DbCommand NewCommand(string commandText, DbConnection connection);
        public abstract DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction);

        public abstract DbDataAdapter NewDataAdapter();
        public abstract DbDataAdapter NewDataAdapter(DbCommand selectCommand);
        public abstract DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection);
        public abstract DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString);
    }
}
