﻿using System.Data;
using System.Data.Common;
using MySqlConnector;

namespace SequelNet.Connector
{
    public class MySql2Factory : IConnectorFactory
    {
        internal static MySql2Factory Shared = new MySql2Factory(null);

        public string ConnectionString { get; set; }

        public MySql2Factory(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public ConnectorBase Connector()
        {
            return new MySql2Connector(this);
        }

        public DbParameter NewParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }

        public DbParameter NewParameter(string name, DbType type, object value)
        {
            MySqlParameter p = new MySqlParameter(name, value);
            p.DbType = type;
            return p;
        }

        public DbParameter NewParameter(string name, object value, ParameterDirection parameterDirection)
        {
            MySqlParameter p = new MySqlParameter(name, value);
            p.Direction = parameterDirection;
            return p;
        }

        public DbParameter NewParameter(string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable,
            byte precision, byte scale,
            string sourceColumn, DataRowVersion sourceVersion,
            object value)
        {
            MySqlParameter p = new MySqlParameter(name, value);
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

        public DbCommand NewCommand()
        {
            return new MySqlCommand();
        }

        public DbCommand NewCommand(string commandText)
        {
            return new MySqlCommand(commandText);
        }

        public DbCommand NewCommand(string commandText, DbConnection connection)
        {
            return new MySqlCommand(commandText, (MySqlConnection)connection);
        }

        public DbCommand NewCommand(string commandText, DbConnection connection, DbTransaction transaction)
        {
            return new MySqlCommand(commandText, (MySqlConnection)connection, (MySqlTransaction)transaction);
        }

        public DbDataAdapter NewDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        public DbDataAdapter NewDataAdapter(DbCommand selectCommand)
        {
            return new MySqlDataAdapter((MySqlCommand)selectCommand);
        }

        public DbDataAdapter NewDataAdapter(string selectCommandText, DbConnection connection)
        {
            return new MySqlDataAdapter(selectCommandText, (MySqlConnection)connection);
        }

        public DbDataAdapter NewDataAdapter(string selectCommandText, string selectConnString)
        {
            return new MySqlDataAdapter(selectCommandText, selectConnString);
        }
    }
}
