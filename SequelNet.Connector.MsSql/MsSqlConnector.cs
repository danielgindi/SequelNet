using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

[assembly: CLSCompliant(true)]

namespace SequelNet.Connector
{
    public class MsSqlConnector : ConnectorBase
    {
        #region Instancing

        private MsSqlFactory _Factory;

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.MSSQL; }
        }

        public static SqlConnection CreateSqlConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public MsSqlConnector(MsSqlFactory factory)
        {
            _Factory = factory;
            Connection = CreateSqlConnection(_Factory.ConnectionString);
        }

        public MsSqlConnector(string connectionString)
        {
            _Factory = MsSqlFactory.Shared;
            Connection = CreateSqlConnection(connectionString);
        }

        ~MsSqlConnector()
        {
            Dispose(false);
        }

        public override IConnectorFactory Factory => _Factory;

        private static ConcurrentDictionary<MsSqlVersion, MsSqlLanguageFactory> _LanguageFactories = new ConcurrentDictionary<MsSqlVersion, MsSqlLanguageFactory>();
        private MsSqlLanguageFactory _LanguageFactory = null;

        public override LanguageFactory Language
        {
            get
            {
                if (_LanguageFactory == null)
                {
                    if (!_LanguageFactories.TryGetValue(GetVersionData(), out var factory))
                    {
                        factory = new MsSqlLanguageFactory(GetVersionData());
                        _LanguageFactories[GetVersionData()] = factory;
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

        static private ConcurrentDictionary<string, MsSqlVersion> _Map_ConnStr_Version = new ConcurrentDictionary<string, MsSqlVersion>();

        private MsSqlVersion? _Version = null;

        public MsSqlVersion GetVersionData()
        {
            if (_Version == null)
            {
                if (_Map_ConnStr_Version.TryGetValue(Connection.ConnectionString, out var version))
                {
                    _Version = version;
                }
                else
                {
                    // Connection string may be altered after connection is opened (persisting without passwords etc.)
                    if (Connection.State == System.Data.ConnectionState.Closed)
                    {
                        Connection.Open(); // Open to get ConnectionString to change to its secure form

                        if (_Map_ConnStr_Version.TryGetValue(Connection.ConnectionString, out version))
                            _Version = version;
                    }

                    if (_Version == null)
                    {
                        version = new MsSqlVersion();

                        try
                        {
                            using (var reader = ExecuteReader("SELECT SERVERPROPERTY('ProductVersion'), SERVERPROPERTY('ProductLevel'), SERVERPROPERTY('Edition')"))
                            {
                                if (reader.Read())
                                {
                                    version.Version = reader.GetStringOrEmpty(0);
                                    version.Level = reader.GetStringOrEmpty(1);
                                    version.Edition = reader.GetStringOrEmpty(2);
                                }
                            }

                            _Version = version;
                            _Map_ConnStr_Version[Connection.ConnectionString] = version;
                        }
                        catch
                        {
                        }

                        return version;
                    }
                }
            }

            return _Version.Value;
        }

        public override string GetVersion()
        {
            return GetVersionData().Version;
        }

        public override bool SupportsSelectPaging()
        {
            return GetVersionData().SupportsOffset;
        }

        public SqlConnection GetUnderlyingConnection()
        {
            return (SqlConnection)Connection;
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"SELECT @@identity AS id");
        }

        public override Task<object> GetLastInsertIdAsync()
        {
            return ExecuteScalarAsync(@"SELECT @@identity AS id");
        }

        public override void SetIdentityInsert(string tableName, bool enabled)
        {
            string sql = string.Format(@"SET IDENTITY_INSERT {0} {1}",
                Language.WrapFieldName(tableName), enabled ? @"ON" : @"OFF");
            ExecuteNonQuery(sql);
        }

        public override bool CheckIfTableExists(string tableName)
        {
            return ExecuteScalar(@"SELECT name FROM sysObjects WHERE name like " + Language.PrepareValue(tableName)) != null;
        }

        public override async Task<bool> CheckIfTableExistsAsync(string tableName)
        {
            return await ExecuteScalarAsync(@"SELECT name FROM sysObjects WHERE name like " + Language.PrepareValue(tableName)) != null;
        }

        #endregion

        #region DB Mutex

        public override bool GetLock(string lockName, TimeSpan timeout, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            string ownerString = "Session";
            switch (owner)
            {
                case SqlMutexOwner.Transaction:
                    ownerString = "Transaction";
                    break;
            }

            SqlCommand sqlCommand = new SqlCommand("sp_getapplock", (SqlConnection)Connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandTimeout = (int)Math.Ceiling(timeout.TotalSeconds);

            sqlCommand.Parameters.AddWithValue("Resource", lockName);
            sqlCommand.Parameters.AddWithValue("LockOwner", ownerString);
            sqlCommand.Parameters.AddWithValue("LockMode", "Exclusive");
            sqlCommand.Parameters.AddWithValue("LockTimeout", (Int32)timeout.TotalMilliseconds);
            sqlCommand.Parameters.AddWithValue("DbPrincipal", dbPrincipal ?? "public");

            SqlParameter returnValue = sqlCommand.Parameters.Add("ReturnValue", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            sqlCommand.ExecuteNonQuery();

            if (Convert.ToInt32(returnValue.Value) < 0)
            {
                return false;
            }

            return true;
        }

        public override bool ReleaseLock(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            string ownerString = "Session";
            switch (owner)
            {
                case SqlMutexOwner.Transaction:
                    ownerString = "Transaction";
                    break;
            }

            SqlCommand sqlCommand = new SqlCommand("sp_releaseapplock", (SqlConnection)Connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("Resource", lockName);
            sqlCommand.Parameters.AddWithValue("LockOwner", ownerString);
            sqlCommand.Parameters.AddWithValue("DbPrincipal", dbPrincipal ?? "public");

            SqlParameter returnValue = sqlCommand.Parameters.Add("ReturnValue", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            sqlCommand.ExecuteNonQuery();

            if (Convert.ToInt32(returnValue.Value) < 0)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
