using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

[assembly: CLSCompliant(true)]

namespace SequelNet.Connector
{
    public class PostgreSQLConnector : ConnectorBase
    {
        #region Instancing

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.POSTGRESQL; }
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
        public static NpgsqlConnection CreateSqlConnection(string connectionStringKey)
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            return new NpgsqlConnection(FindConnectionString(connectionStringKey));
        }
        
        public PostgreSQLConnector()
        {
            Connection = CreateSqlConnection(null);
        }

        public PostgreSQLConnector(string connectionStringKey)
        {
            Connection = CreateSqlConnection(connectionStringKey);
        }

        ~PostgreSQLConnector()
        {
            Dispose(false);
        }

        public override IConnectorFactory Factory => PostgreSQLFactory.Instance;

        private static ConcurrentDictionary<PostgreSQLMode, PostgreSQLLanguageFactory> _LanguageFactories = new ConcurrentDictionary<PostgreSQLMode, PostgreSQLLanguageFactory>();
        private PostgreSQLLanguageFactory _LanguageFactory = null;

        public override LanguageFactory Language
        {
            get
            {
                if (_LanguageFactory == null)
                {
                    if (!_LanguageFactories.TryGetValue(GetPostgreSQLMode(), out var factory))
                    {
                        factory = new PostgreSQLLanguageFactory(GetPostgreSQLMode());
                        _LanguageFactories[GetPostgreSQLMode()] = factory;
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

        static private ConcurrentDictionary<string, PostgreSQLMode> _Map_ConnStr_SqlMode = new ConcurrentDictionary<string, PostgreSQLMode>();

        private PostgreSQLMode? _PostgreSQLMode = null;

        public PostgreSQLMode GetPostgreSQLMode()
        {
            if (_PostgreSQLMode == null)
            {
                if (_Map_ConnStr_SqlMode.TryGetValue(Connection.ConnectionString, out var sqlMode))
                {
                    _PostgreSQLMode = sqlMode;
                }
                else
                {
                    // Connection string may be altered after connection is opened (persisting without passwords etc.)
                    if (Connection.State == System.Data.ConnectionState.Closed)
                    {
                        Connection.Open(); // Open to get ConnectionString to change to its secure form

                        if (_Map_ConnStr_SqlMode.TryGetValue(Connection.ConnectionString, out sqlMode))
                            _PostgreSQLMode = sqlMode;
                    }

                    if (_PostgreSQLMode == null)
                    {
                        sqlMode = new PostgreSQLMode();

                        try
                        {
                            sqlMode.StandardConformingStrings = ExecuteScalar("show standard_conforming_strings").ToString() == @"on";
                        }
                        catch { }

                        try
                        {
                            sqlMode.BackslashQuote = ExecuteScalar("show backslash_quote").ToString() == @"on";
                        }
                        catch { }

                        try
                        {
                            sqlMode.Version = ExecuteScalar("select version()").ToString();
                        }
                        catch { }

                        _Map_ConnStr_SqlMode[Connection.ConnectionString] = sqlMode;
                        _PostgreSQLMode = sqlMode;
                    }
                }
            }

            return _PostgreSQLMode.Value;
        }

        public override string GetVersion()
        {
            return GetPostgreSQLMode().Version;
        }

        public override bool SupportsSelectPaging()
        {
            return true;
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
        public NpgsqlConnection GetUnderlyingConnection()
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            return (NpgsqlConnection)Connection;
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar("select lastval() AS id");
        }

        public override void SetIdentityInsert(string tableName, bool enabled)
        {
            // Nothing to do. In PostgreSQL IDENTITY_INSERT is always allowed
        }

        public override bool CheckIfTableExists(string tableName)
        {
            return ExecuteScalar($"select * from information_schema.tables where table_name= {Language.PrepareValue(tableName)}") != null;
        }

        public override async Task<bool> CheckIfTableExistsAsync(string tableName)
        {
            return await ExecuteScalarAsync($"select * from information_schema.tables where table_name= {Language.PrepareValue(tableName)}") != null;
        }

        #endregion
    }
}
