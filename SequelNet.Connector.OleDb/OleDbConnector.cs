using System;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]

namespace SequelNet.Connector
{
    public class OleDbConnector : ConnectorBase
    {
        #region Instancing

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.MSACCESS; }
        }

        public static OleDbConnection CreateSqlConnection(string connectionStringKey)
        {
            return new OleDbConnection(FindConnectionString(connectionStringKey));
        }

        public OleDbConnector()
        {
            Connection = CreateSqlConnection(null);
        }

        public OleDbConnector(string connectionStringKey)
        {
            Connection = CreateSqlConnection(connectionStringKey);
        }

        ~OleDbConnector()
        {
            Dispose(false);
        }

        public override IConnectorFactory Factory => OleDbFactory.Instance;

        private static OleDbLanguageFactory _LanguageFactory = new OleDbLanguageFactory();

        public override LanguageFactory Language => _LanguageFactory;

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

        public override bool SupportsSelectPaging()
        {
            return false;
        }

        public OleDbConnection GetUnderlyingConnection()
        {
            return (OleDbConnection)Connection;
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar("SELECT @@identity AS id");
        }

        public override bool CheckIfTableExists(string TableName)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            return ExecuteScalar($"SELECT name FROM MSysObjects WHERE name like {Language.PrepareValue(TableName)}") != null;
        }

        #endregion
    }
}
