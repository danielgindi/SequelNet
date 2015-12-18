using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Text;
using dg.Sql.Sql.Spatial;
using System.Globalization;

namespace dg.Sql.Connector
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

        private MySqlConnection _Connection = null;

        public MySqlConnector()
        {
            _Connection = CreateSqlConnection(null);
        }
        public MySqlConnector(string connectionStringKey)
        {
            _Connection = CreateSqlConnection(connectionStringKey);
        }
        ~MySqlConnector()
        {
            Dispose(false);
        }

        public override void Close()
        {
            try
            {
                if (_Connection != null && _Connection.State != ConnectionState.Closed)
                {
                    _Connection.Close();
                }
            }
            catch { }
            if (_Connection != null) _Connection.Dispose();
            _Connection = null;
        }

        public override DbConnection Connection
        {
            get { return _Connection; }
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
            // Now clean up Native Resources (Pointers)
        }

        #endregion

        #region Executing

        public override int ExecuteNonQuery(string QuerySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (MySqlCommand command = new MySqlCommand(QuerySql, _Connection, _Transaction))
            {
                return command.ExecuteNonQuery();
            }
        }
        public override int ExecuteNonQuery(DbCommand Command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            Command.Connection = _Connection;
            Command.Transaction = _Transaction;
            return Command.ExecuteNonQuery();
        }
        public override object ExecuteScalar(string QuerySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (MySqlCommand command = new MySqlCommand(QuerySql, _Connection, _Transaction))
            {
                return command.ExecuteScalar();
            }
        }
        public override object ExecuteScalar(DbCommand Command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            Command.Connection = _Connection;
            Command.Transaction = _Transaction;
            return Command.ExecuteScalar();
        }
        public override DataReaderBase ExecuteReader(string QuerySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (MySqlCommand command = new MySqlCommand(QuerySql, _Connection, _Transaction))
            {
                return new MySqlDataReader(command.ExecuteReader());
            }
        }
        public override DataReaderBase ExecuteReader(string QuerySql, bool AttachConnectionToReader)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (MySqlCommand command = new MySqlCommand(QuerySql, _Connection, _Transaction))
            {
                return new MySqlDataReader(command.ExecuteReader(), AttachConnectionToReader ? this : null);
            }
        }
        public override DataReaderBase ExecuteReader(DbCommand Command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            Command.Connection = _Connection;
            Command.Transaction = _Transaction;
            return new MySqlDataReader(((MySqlCommand)Command).ExecuteReader());
        }
        public override DataReaderBase ExecuteReader(DbCommand Command, bool AttachConnectionToReader)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            Command.Connection = _Connection;
            Command.Transaction = _Transaction;
            return new MySqlDataReader(((MySqlCommand)Command).ExecuteReader(), AttachConnectionToReader ? this : null);
        }
        public override DataSet ExecuteDataSet(string QuerySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (MySqlCommand cmd = new MySqlCommand(QuerySql, _Connection, _Transaction))
            {
                DataSet dataSet = new DataSet();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dataSet);
                }
                return dataSet;
            }
        }
        public override DataSet ExecuteDataSet(DbCommand Command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            Command.Connection = _Connection;
            Command.Transaction = _Transaction;
            DataSet dataSet = new DataSet();
            using (MySqlDataAdapter adapter = new MySqlDataAdapter((MySqlCommand)Command))
            {
                adapter.Fill(dataSet);
            }
            return dataSet;
        }
        public override int ExecuteScript(string QuerySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            MySqlScript script = new MySqlScript(_Connection, QuerySql);
            return script.Execute();
        }

        #endregion

        #region Utilities

        static private Dictionary<string, MySqlMode> _Map_ConnStr_SqlMode = new Dictionary<string, MySqlMode>();
        static private Dictionary<string, string> _Map_ConnStr_Version = new Dictionary<string, string>();

        private MySqlMode _MySqlMode = null;
        private string _Version = null;

        public MySqlMode GetMySqlMode()
        {
            if (_MySqlMode == null)
            {
                MySqlMode sqlMode;
                if (_Map_ConnStr_SqlMode.TryGetValue(_Connection.ConnectionString, out sqlMode))
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
                    _Map_ConnStr_SqlMode[_Connection.ConnectionString] = sqlMode;
                    _MySqlMode = sqlMode;
                }
            }
            return _MySqlMode;
        }

        public override string GetVersion()
        {
            if (_Version == null)
            {
                string version;
                if (_Map_ConnStr_Version.TryGetValue(_Connection.ConnectionString, out version))
                {
                    _Version = version;
                }
                else
                {
                    try
                    {
                        version = ExecuteScalar("SELECT @@VERSION").ToString();
                        _Version = version;
                        _Map_ConnStr_Version[_Connection.ConnectionString] = _Version;
                    }
                    catch { }
                }
            }
            return _Version;
        }

        public override bool SupportsSelectPaging()
        {
            return true;
        }

        public MySqlConnection GetUnderlyingConnection()
        {
            return _Connection;
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"SELECT LAST_INSERT_ID() AS id");
        }

        public override void SetIdentityInsert(string TableName, bool Enabled)
        {
            // Nothing to do. In MySql IDENTITY_INSERT is always allowed
        }

        public override bool CheckIfTableExists(string TableName)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            return ExecuteScalar(@"SHOW TABLES LIKE " + PrepareValue(TableName)) != null;
        }

        #endregion

        #region Transactions

        MySqlTransaction _Transaction = null;
        Stack<MySqlTransaction> _Transactions = null;

        public override bool BeginTransaction()
        {
            try
            {
                if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction();
                if (_Transactions == null) _Transactions = new Stack<MySqlTransaction>(1);
                _Transactions.Push(_Transaction);
                return (_Transaction != null);
            }
            catch (MySqlException) { }
            return false;
        }

        public override bool BeginTransaction(IsolationLevel IsolationLevel)
        {
            try
            {
                if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction(IsolationLevel);
                if (_Transactions == null) _Transactions = new Stack<MySqlTransaction>(1);
                _Transactions.Push(_Transaction);
            }
            catch (MySqlException) { return false; }
            return (_Transaction != null);
        }

        public override bool CommitTransaction()
        {
            if (_Transaction == null) return false;
            else
            {
                try
                {
                    _Transaction.Commit();
                }
                catch (MySqlException) { return false; }
                _Transactions.Pop();
                if (_Transactions.Count > 0) _Transaction = _Transactions.Peek();
                else _Transaction = null;
                return true;
            }
        }

        public override bool RollbackTransaction()
        {
            if (_Transaction == null) return false;
            else
            {
                try
                {
                    _Transaction.Rollback();
                }
                catch (MySqlException) { return false; }
                _Transactions.Pop();
                if (_Transactions.Count > 0) _Transaction = _Transactions.Peek();
                else _Transaction = null;
                return true;
            }
        }

        public override bool HasTransaction
        {
            get { return _Transactions != null && _Transactions.Count > 0; }
        }

        public override int CurrentTransactions
        {
            get { return _Transactions == null ? 0 : _Transactions.Count; }
        }

        public override DbTransaction Transaction
        {
            get { return _Transaction; }
        }

        #endregion

        #region Preparing values for SQL

        public override string EncloseFieldName(string FieldName)
        { // Note: For performance, ignoring enclosed ` signs
            return '`' + FieldName + '`';
        }


        private static string CharactersNeedsBackslashes = // Other special characters for escaping
            "\u005c\u00a5\u0160\u20a9\u2216\ufe68\uff3c";
        private static string CharactersNeedsDoubling = // Kinds of quotes...
            "\u0027\u0060\u00b4\u02b9\u02ba\u02bb\u02bc\u02c8\u02ca\u02cb\u02d9\u0300\u0301\u2018\u2019\u201a\u2032\u2035\u275b\u275c\uff07";

        public static string EscapeStringWithBackslashes(string Value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in Value)
            {
                if (CharactersNeedsDoubling.IndexOf(c) >= 0 || CharactersNeedsBackslashes.IndexOf(c) >= 0)
                {
                    sb.Append("\\");
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string EscapeStringWithoutBackslashes(string Value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in Value)
            {
                if (CharactersNeedsDoubling.IndexOf(c) >= 0)
                {
                    sb.Append(c);
                }
                else if (CharactersNeedsBackslashes.IndexOf(c) >= 0)
                {
                    sb.Append("\\");
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public override string EscapeString(string Value)
        {
            if (GetMySqlMode().NoBackSlashes)
            {
                return EscapeStringWithoutBackslashes(Value);
            }
            else
            {
                return EscapeStringWithBackslashes(Value);
            }
        }

        public override string PrepareValue(Guid Value)
        {
            return '\'' + Value.ToString(@"D") + '\'';
        }

        public override string FormatDate(DateTime DateTime)
        {
            return DateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string Expression)
        {
            return Expression.Replace(@"'", @"''").Replace(@"%", @"%%");
        }
        public override string LikeEscapingStatement
        {
            get { return @"ESCAPE('\\')"; }
        }

        #endregion

        #region Reading values from SQL

        public override Geometry ReadGeometry(object Value)
        {
            byte[] geometryData = Value as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, true);
            }
            return null;
        }

        #endregion

        #region Engine-specific keywords

        public override int varchar_MAX_VALUE
        {
            get 
            {
                if (GetVersion().CompareTo("5.0.3") >= 0)
                {
                    return 21845;
                }
                else
                {
                    return 255;
                }
            }
        }

        public override string func_UTC_NOW
        {
            get { return @"UTC_TIMESTAMP()"; }
        }

        public override string type_AUTOINCREMENT { get { return @"AUTO_INCREMENT"; } }
        public override string type_AUTOINCREMENT_BIGINT { get { return @"AUTO_INCREMENT"; } }

        public override string type_TINYINT { get { return @"TINYINT"; } }
        public override string type_UNSIGNEDTINYINT { get { return @"TINYINT UNSIGNED"; } }
        public override string type_SMALLINT { get { return @"SMALLINT"; } }
        public override string type_UNSIGNEDSMALLINT { get { return @"SMALLINT UNSIGNED"; } }
        public override string type_INT { get { return @"INT"; } }
        public override string type_UNSIGNEDINT { get { return @"INT UNSIGNED"; } }
        public override string type_BIGINT { get { return @"BIGINT"; } }
        public override string type_UNSIGNEDBIGINT { get { return @"BIGINT UNSIGNED"; } }
        public override string type_NUMERIC { get { return @"NUMERIC"; } }
        public override string type_DECIMAL { get { return @"DECIMAL"; } }
        public override string type_MONEY { get { return @"DECIMAL"; } }
        public override string type_FLOAT { get { return @"FLOAT"; } }
        public override string type_DOUBLE { get { return @"DOUBLE"; } }
        public override string type_VARCHAR { get { return @"NATIONAL VARCHAR"; } }
        public override string type_CHAR { get { return @"NATIONAL CHAR"; } }
        public override string type_TEXT { get { return @"TEXT"; } }
        public override string type_MEDIUMTEXT { get { return @"MEDIUMTEXT"; } }
        public override string type_LONGTEXT { get { return @"LONGTEXT"; } }
        public override string type_BOOLEAN { get { return @"BOOLEAN"; } }
        public override string type_DATETIME { get { return @"DATETIME"; } }
        public override string type_BLOB { get { return @"BLOB"; } }
        public override string type_GUID { get { return @"NATIONAL CHAR(36)"; } }

        public override string type_GEOMETRY { get { return @"GEOMETRY"; } }
        public override string type_GEOMETRYCOLLECTION { get { return @"GEOMETRYCOLLECTION"; } }
        public override string type_POINT { get { return @"POINT"; } }
        public override string type_LINESTRING { get { return @"LINESTRING"; } }
        public override string type_POLYGON { get { return @"POLYGON"; } }
        public override string type_LINE { get { return @"LINE"; } }
        public override string type_CURVE { get { return @"CURVE"; } }
        public override string type_SURFACE { get { return @"SURFACE"; } }
        public override string type_LINEARRING { get { return @"LINEARRING"; } }
        public override string type_MULTIPOINT { get { return @"MULTIPOINT"; } }
        public override string type_MULTILINESTRING { get { return @"MULTILINESTRING"; } }
        public override string type_MULTIPOLYGON { get { return @"MULTIPOLYGON"; } }
        public override string type_MULTICURVE { get { return @"MULTICURVE"; } }
        public override string type_MULTISURFACE { get { return @"MULTISURFACE"; } }

        public override string type_GEOGRAPHIC { get { return @"GEOMETRY"; } }
        public override string type_GEOGRAPHICCOLLECTION { get { return @"GEOMETRYCOLLECTION"; } }
        public override string type_GEOGRAPHIC_POINT { get { return @"POINT"; } }
        public override string type_GEOGRAPHIC_LINESTRING { get { return @"LINESTRING"; } }
        public override string type_GEOGRAPHIC_POLYGON { get { return @"POLYGON"; } }
        public override string type_GEOGRAPHIC_LINE { get { return @"LINE"; } }
        public override string type_GEOGRAPHIC_CURVE { get { return @"CURVE"; } }
        public override string type_GEOGRAPHIC_SURFACE { get { return @"SURFACE"; } }
        public override string type_GEOGRAPHIC_LINEARRING { get { return @"LINEARRING"; } }
        public override string type_GEOGRAPHIC_MULTIPOINT { get { return @"MULTIPOINT"; } }
        public override string type_GEOGRAPHIC_MULTILINESTRING { get { return @"MULTILINESTRING"; } }
        public override string type_GEOGRAPHIC_MULTIPOLYGON { get { return @"MULTIPOLYGON"; } }
        public override string type_GEOGRAPHIC_MULTICURVE { get { return @"MULTICURVE"; } }
        public override string type_GEOGRAPHIC_MULTISURFACE { get { return @"MULTISURFACE"; } }

        #endregion

        #region DB Mutex

        public virtual bool GetLock(string lockName, TimeSpan timeout, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            object sqlLock = ExecuteScalar(string.Format("SELECT GET_LOCK('{0}', {1})",
                EscapeString(lockName), (timeout == TimeSpan.MaxValue ? "-1" : timeout.TotalSeconds.ToString(CultureInfo.InvariantCulture))));

            if (sqlLock == null || 
                sqlLock == DBNull.Value || 
                (sqlLock is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)sqlLock).IsNull) ||
                Convert.ToInt32(sqlLock) != 1)
            {
                return false;
            }

            return true;
        }

        public bool ReleaseLock(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            object sqlLock = ExecuteScalar(@"SELECT RELEASE_LOCK('" + EscapeString(lockName) + "')");
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
