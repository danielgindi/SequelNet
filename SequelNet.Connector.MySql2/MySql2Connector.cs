﻿using System;
using System.Globalization;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using MySqlConnector;

[assembly: CLSCompliant(true)]

namespace SequelNet.Connector
{
    public class MySql2Connector : ConnectorBase
    {
        #region Instancing

        private MySql2Factory _Factory;

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.MYSQL; }
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
        public static MySqlConnection CreateSqlConnection(string connectionString)
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            return new MySqlConnection(connectionString);
        }

        public MySql2Connector(MySql2Factory factory)
        {
            _Factory = factory;
            Connection = CreateSqlConnection(_Factory.ConnectionString);
        }

        public MySql2Connector(string connectionString)
        {
            _Factory = MySql2Factory.Shared;
            Connection = CreateSqlConnection(connectionString);
        }

        ~MySql2Connector()
        {
            Dispose(false);
        }

        public override IConnectorFactory Factory => _Factory;

        private static ConcurrentDictionary<MySql2Mode, MySql2LanguageFactory> _LanguageFactories = new ConcurrentDictionary<MySql2Mode, MySql2LanguageFactory>();
        private MySql2LanguageFactory _LanguageFactory = null;

        public override LanguageFactory Language
        {
            get
            {
                if (_LanguageFactory == null)
                {
                    if (!_LanguageFactories.TryGetValue(GetMySqlMode(), out var factory))
                    {
                        factory = new MySql2LanguageFactory(GetMySqlMode());
                        _LanguageFactories[GetMySqlMode()] = factory;
                    }

                    _LanguageFactory = factory;
                }

                return _LanguageFactory;
            }
        }

        #endregion

        #region Executing

        public override int ExecuteScript(string querySql)
        {
            return ExecuteNonQuery(querySql);
        }

        public override Task<int> ExecuteScriptAsync(string querySql, CancellationToken? cancellationToken = null)
        {
            return ExecuteNonQueryAsync(querySql, cancellationToken);
        }

        #endregion

        #region Utilities

        static private ConcurrentDictionary<string, MySql2Mode> _Map_ConnStr_SqlMode = new ConcurrentDictionary<string, MySql2Mode>();

        private MySql2Mode? _MySqlMode = null;

        public MySql2Mode GetMySqlMode()
        {
            if (_MySqlMode == null)
            {
                if (_Map_ConnStr_SqlMode.TryGetValue(Connection.ConnectionString, out var sqlMode))
                {
                    _MySqlMode = sqlMode;
                }
                else
                {
                    // Connection string may be altered after connection is opened (persisting without passwords etc.)
                    if (Connection.State == System.Data.ConnectionState.Closed)
                    {
                        Connection.Open(); // Open to get ConnectionString to change to its secure form

                        if (_Map_ConnStr_SqlMode.TryGetValue(Connection.ConnectionString, out sqlMode))
                            _MySqlMode = sqlMode;
                    }

                    if (_MySqlMode == null)
                    {
                        sqlMode = new MySql2Mode();

                        try
                        {
                            sqlMode.SqlMode = ExecuteScalar("SELECT @@SQL_MODE").ToString();
                        }
                        catch { }

                        try
                        {
                            sqlMode.Version = ExecuteScalar("SELECT @@VERSION").ToString();
                        }
                        catch { }

                        _Map_ConnStr_SqlMode[Connection.ConnectionString] = sqlMode;
                        _MySqlMode = sqlMode;
                    }
                }
            }

            return _MySqlMode.Value;
        }

        public override string GetVersion()
        {
            return GetMySqlMode().Version;
        }

        public override bool SupportsSelectPaging()
        {
            return true;
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
        public MySqlConnection GetUnderlyingConnection()
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            return (MySqlConnection)Connection;
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"SELECT LAST_INSERT_ID() AS id");
        }

        public override Task<object> GetLastInsertIdAsync()
        {
            return ExecuteScalarAsync(@"SELECT LAST_INSERT_ID() AS id");
        }

        public override void SetIdentityInsert(string tableName, bool enabled)
        {
            // Nothing to do. In MySql IDENTITY_INSERT is always allowed
        }

        public override bool CheckIfTableExists(string tableName)
        {
            return ExecuteScalar(@"SHOW TABLES LIKE " + Language.PrepareValue(tableName)) != null;
        }

        public override async Task<bool> CheckIfTableExistsAsync(string tableName)
        {
            return await ExecuteScalarAsync(@"SHOW TABLES LIKE " + Language.PrepareValue(tableName)) != null;
        }

        #endregion

        #region DB Mutex

        public override bool GetLock(string lockName, TimeSpan timeout, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            object sqlLock = ExecuteScalar(
                string.Format("SELECT GET_LOCK({0}, {1})",
                Language.PrepareValue(lockName),
                (timeout == TimeSpan.MaxValue ? "-1" : timeout.TotalSeconds.ToString(CultureInfo.InvariantCulture))));

            if (sqlLock == null ||
                sqlLock == DBNull.Value ||
                (sqlLock is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)sqlLock).IsNull) ||
                Convert.ToInt32(sqlLock) != 1)
            {
                return false;
            }

            return true;
        }

        public override bool IsLockUsed(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            object sqlLock = ExecuteScalar($"SELECT IS_USED_LOCK({Language.PrepareValue(lockName)})");

            if (sqlLock == null ||
                sqlLock == DBNull.Value ||
                (sqlLock is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)sqlLock).IsNull) ||
                Convert.ToInt32(sqlLock) != 1)
            {
                return false;
            }

            return true;
        }

        public override bool IsLockFree(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            object sqlLock = ExecuteScalar($"SELECT IS_FREE_LOCK({Language.PrepareValue(lockName)})");

            if (sqlLock == null ||
                sqlLock == DBNull.Value ||
                (sqlLock is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)sqlLock).IsNull) ||
                Convert.ToInt32(sqlLock) != 1)
            {
                return false;
            }

            return true;
        }

        public override bool ReleaseLock(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            object sqlLock = ExecuteScalar($"SELECT RELEASE_LOCK({Language.PrepareValue(lockName)})");
            if (sqlLock == null ||
                sqlLock == DBNull.Value ||
                (sqlLock is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)sqlLock).IsNull) ||
                Convert.ToInt32(sqlLock) != 1)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
