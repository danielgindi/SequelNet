using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
{
    public class MsSqlConnector : ConnectorBase
    {        
        #region Instancing

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.MSSQL; }
        }

        public static SqlConnection CreateSqlConnection(string connectionStringKey)
        {
            return new SqlConnection(FindConnectionString(connectionStringKey));
        }

        public MsSqlConnector()
        {
            Connection = CreateSqlConnection(null);
        }

        public MsSqlConnector(string connectionStringKey)
        {
            Connection = CreateSqlConnection(connectionStringKey);
        }

        ~MsSqlConnector()
        {
            Dispose(false);
        }

        public override FactoryBase Factory => MsSqlFactory.Instance;

        private static Dictionary<MsSqlVersion, MsSqlLanguageFactory> _LanguageFactories = new Dictionary<MsSqlVersion, MsSqlLanguageFactory>();
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
            throw new NotImplementedException(@"ExecuteScript");
        }

        #endregion

        #region Utilities

        static private Dictionary<string, MsSqlVersion> _Map_ConnStr_Version = new Dictionary<string, MsSqlVersion>();

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

        public override void SetIdentityInsert(string TableName, bool Enabled)
        {
            string sql = string.Format(@"SET IDENTITY_INSERT {0} {1}",
                Language.WrapFieldName(TableName), Enabled ? @"ON" : @"OFF");
            ExecuteNonQuery(sql);
        }

        public override bool CheckIfTableExists(string TableName)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            return ExecuteScalar(@"SELECT name FROM sysObjects WHERE name like " + Language.PrepareValue(TableName)) != null;
        }

        #endregion

        #region Reading values from SQL

        public override Geometry ReadGeometry(object value)
        {
            byte[] geometryData = value as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, false);
            }
            return null;
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
