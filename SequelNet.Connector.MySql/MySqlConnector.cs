using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace SequelNet.Connector
{
    public class MySqlConnector : ConnectorBase
    {
        #region Instancing

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.MYSQL; }
        }

        public static MySqlConnection CreateSqlConnection(string connectionStringKey)
        {
            return new MySqlConnection(FindConnectionString(connectionStringKey));
        }

        public MySqlConnector()
        {
            Connection = CreateSqlConnection(null);
        }

        public MySqlConnector(string connectionStringKey)
        {
            Connection = CreateSqlConnection(connectionStringKey);
        }

        ~MySqlConnector()
        {
            Dispose(false);
        }

        public override IConnectorFactory Factory => MySqlFactory.Instance;

        private static Dictionary<MySqlMode, MySqlLanguageFactory> _LanguageFactories = new Dictionary<MySqlMode, MySqlLanguageFactory>();
        private MySqlLanguageFactory _LanguageFactory = null;

        public override LanguageFactory Language
        {
            get
            {
                if (_LanguageFactory == null)
                {
                    if (!_LanguageFactories.TryGetValue(GetMySqlMode(), out var factory))
                    {
                        factory = new MySqlLanguageFactory(GetMySqlMode());
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
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            MySqlScript script = new MySqlScript((MySqlConnection)Connection, querySql);
            return script.Execute();
        }

        #endregion

        #region Utilities

        static private Dictionary<string, MySqlMode> _Map_ConnStr_SqlMode = new Dictionary<string, MySqlMode>();

        private MySqlMode? _MySqlMode = null;

        public MySqlMode GetMySqlMode()
        {
            if (_MySqlMode == null)
            {
                if (_Map_ConnStr_SqlMode.TryGetValue(Connection.ConnectionString, out var sqlMode))
                {
                    _MySqlMode = sqlMode;
                }
                else
                {
                    sqlMode = new MySqlMode();

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

        public MySqlConnection GetUnderlyingConnection()
        {
            return (MySqlConnection)Connection;
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"SELECT LAST_INSERT_ID() AS id");
        }

        public override void SetIdentityInsert(string tableName, bool enabled)
        {
            // Nothing to do. In MySql IDENTITY_INSERT is always allowed
        }

        public override bool CheckIfTableExists(string tableName)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            return ExecuteScalar(@"SHOW TABLES LIKE " + Language.PrepareValue(tableName)) != null;
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
