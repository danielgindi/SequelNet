using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Text;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
{
    public class MySqlMode
    {
        private string _sqlMode = null;
        private bool? _noBackSlashes = null;
        private bool? _ansiQuotes = null;

        public string SqlMode
        {
            get { return _sqlMode; }
            set { _sqlMode = value; }
        }
        public bool NoBackSlashes
        {
            get
            {
                if (_noBackSlashes == null)
                {
                    if (_sqlMode == null) return false;
                    _noBackSlashes = _sqlMode.IndexOf("NO_BACKSLASH_ESCAPES") != -1;
                }
                return _noBackSlashes == true;
            }
            set { _noBackSlashes = value; }
        }
        public bool AnsiQuotes
        {
            get
            {
                if (_ansiQuotes == null)
                {
                    if (_sqlMode == null) return false;
                    _ansiQuotes = _sqlMode.IndexOf("ANSI_QUOTES") != -1;
                }
                return _ansiQuotes == true;
            }
            set { _ansiQuotes = value; }
        }
    }

    public class MySqlConnector : ConnectorBase
    {
        static private Dictionary<string, MySqlMode> map_ConnStr_SqlMode = new Dictionary<string, MySqlMode>();
        static private Dictionary<string, string> map_ConnStr_Version = new Dictionary<string, string>();
        private MySqlMode _mySqlMode = null;
        private string _version = null;

        MySqlTransaction _transaction = null;
        Stack<MySqlTransaction> _transactions = null;

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.MYSQL; }
        }

        public static MySqlConnection CreateSqlConnection(string connectionStringKey)
        {
            return new MySqlConnection(GetWebsiteConnectionString(connectionStringKey));
        }

        MySqlConnection _conn = null;

        public MySqlConnector()
        {
            _conn = CreateSqlConnection(null);
        }
        public MySqlConnector(string connectionStringKey)
        {
            _conn = CreateSqlConnection(connectionStringKey);
        }
        ~MySqlConnector()
        {
            Dispose(false);
        }

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

        public override void Close()
        {
            try
            {
                if (_conn != null && _conn.State != ConnectionState.Closed)
                {
                    _conn.Close();
                }
            }
            catch (Exception) { }
            if (_conn != null) _conn.Dispose();
            _conn = null;
        }
        public MySqlConnection GetConn()
        {
            return _conn;
        }
        public override int ExecuteNonQuery(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return new MySqlCommand(strSQL, _conn, _transaction).ExecuteNonQuery();
        }
        public override int ExecuteNonQuery(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return command.ExecuteNonQuery();
        }
        public override object ExecuteScalar(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return new MySqlCommand(strSQL, _conn, _transaction).ExecuteScalar();
        }
        public override object ExecuteScalar(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return command.ExecuteScalar();
        }
        public override DataReaderBase ExecuteReader(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return new MySqlDataReader(
                new MySqlCommand(strSQL, _conn, _transaction).ExecuteReader());
        }
        public override DataReaderBase ExecuteReader(String strSQL, bool attachConnectionToReader)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return new MySqlDataReader(
                new MySqlCommand(strSQL, _conn, _transaction).ExecuteReader(), attachConnectionToReader ? this : null);
        }
        public override DataReaderBase ExecuteReader(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return new MySqlDataReader(((MySqlCommand)command).ExecuteReader());
        }
        public override DataReaderBase ExecuteReader(DbCommand command, bool attachConnectionToReader)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return new MySqlDataReader(((MySqlCommand)command).ExecuteReader(), attachConnectionToReader ? this : null);
        }
        public override DataSet ExecuteDataSet(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            using (MySqlCommand cmd = new MySqlCommand(strSQL, _conn, _transaction))
            {
                DataSet dataSet = new DataSet();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dataSet);
                }
                return dataSet;
            }
        }
        public override DataSet ExecuteDataSet(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            DataSet dataSet = new DataSet();
            using (MySqlDataAdapter adapter = new MySqlDataAdapter((MySqlCommand)command))
            {
                adapter.Fill(dataSet);
            }
            return dataSet;
        }
        public override int ExecuteScript(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            MySqlScript script = new MySqlScript(_conn, strSQL);
            return script.Execute();
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"SELECT LAST_INSERT_ID() AS id");
        }

        public MySqlMode GetMySqlMode()
        {
            if (_mySqlMode == null)
            {
                MySqlMode sqlMode;
                if (map_ConnStr_SqlMode.TryGetValue(_conn.ConnectionString, out sqlMode))
                {
                    _mySqlMode = sqlMode;
                }
                else
                {
                    sqlMode = new MySqlMode();
                    try
                    {
                        sqlMode.SqlMode = ExecuteScalar("SELECT @@SQL_MODE").ToString();
                    }
                    catch { }
                    map_ConnStr_SqlMode[_conn.ConnectionString] = sqlMode;
                    _mySqlMode = sqlMode;
                }
            }
            return _mySqlMode;
        }
        public override string GetVersion()
        {
            if (_version == null)
            {
                string version;
                if (map_ConnStr_Version.TryGetValue(_conn.ConnectionString, out version))
                {
                    _version = version;
                }
                else
                {
                    try
                    {
                        version = ExecuteScalar("SELECT @@VERSION").ToString();
                        _version = version;
                        map_ConnStr_Version[_conn.ConnectionString] = _version;
                    }
                    catch { }
                }
            }
            return _version;
        }

        public override bool checkIfTableExists(string tableName)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return ExecuteScalar(@"SHOW TABLES LIKE '" + fullEscape(tableName) + "'") != null;
        }

        public override bool beginTransaction()
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
                _transaction = _conn.BeginTransaction();
                if (_transactions == null) _transactions = new Stack<MySqlTransaction>(1);
                _transactions.Push(_transaction);
                return (_transaction != null);
            }
            catch (MySqlException) { }
            return false;
        }
        public override bool beginTransaction(IsolationLevel isolationLevel)
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
                _transaction = _conn.BeginTransaction(isolationLevel);
                if (_transactions == null) _transactions = new Stack<MySqlTransaction>(1);
                _transactions.Push(_transaction);
            }
            catch (MySqlException) { return false; }
            return (_transaction != null);
        }
        public override bool commitTransaction()
        {
            if (_transaction == null) return false;
            else
            {
                try
                {
                    _transaction.Commit();
                }
                catch (MySqlException) { return false; }
                _transactions.Pop();
                if (_transactions.Count > 0) _transaction = _transactions.Peek();
                else _transaction = null;
                return true;
            }
        }
        public override bool rollbackTransaction()
        {
            if (_transaction == null) return false;
            else
            {
                try
                {
                    _transaction.Rollback();
                }
                catch (MySqlException) { return false; }
                _transactions.Pop();
                if (_transactions.Count > 0) _transaction = _transactions.Peek();
                else _transaction = null;
                return true;
            }
        }
        public override bool hasTransaction
        {
            get { return _transactions != null && _transactions.Count > 0; }
        }
        public override int currentTransactions
        {
            get { return _transactions == null ? 0 : _transactions.Count; }
        }
        public override DbTransaction Transaction
        {
            get { return _transaction; }
        }
        public override DbConnection Connection
        {
            get { return _conn; }
        }

        private static string stringOfBackslashChars =
            "\u005c\u00a5\u0160\u20a9\u2216\ufe68\uff3c";
        private static string stringOfQuoteChars =
            "\u0027\u0060\u00b4\u02b9\u02ba\u02bb\u02bc\u02c8\u02ca\u02cb\u02d9\u0300\u0301\u2018\u2019\u201a\u2032\u2035\u275b\u275c\uff07";

        public static string EscapeString(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (stringOfQuoteChars.IndexOf(c) >= 0 ||
                    stringOfBackslashChars.IndexOf(c) >= 0)
                    sb.Append("\\");
                sb.Append(c);
            }
            return sb.ToString();
        }
        public static string DoubleQuoteString(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (stringOfQuoteChars.IndexOf(c) >= 0)
                    sb.Append(c);
                else if (stringOfBackslashChars.IndexOf(c) >= 0)
                    sb.Append("\\");
                sb.Append(c);
            }
            return sb.ToString();
        }

        public override string fullEscape(string strToEscape)
        {
            if (GetMySqlMode().NoBackSlashes)
            {
                return DoubleQuoteString(strToEscape);
            }
            else
            {
                return EscapeString(strToEscape);
            }
        }
        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }
        public override string encloseFieldName(string fieldName)
        {
            return '`' + fieldName + '`';
        }
        public override string formatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }
        public override string sqlAddPaginationAndOrdering(
            string selectFieldsList,
            string primaryKeysList,
            string tablesList,
            string where,
            int limit /* = 0 */, int offset /* = 0 */,
            string orderBy /* = NULL */)
        {
            // The most standard technique

            tablesList = tablesList.TrimStart(new char[] { ' ' });
            if (tablesList.Length > 0) tablesList = @" FROM " + tablesList;
            where = where.TrimStart(new char[] { ' ' });
            if (where.Length > 0) where = @" WHERE " + where;
            if (orderBy == null) orderBy = string.Empty;
            orderBy = orderBy.TrimStart(new char[] { ' ' });
            if (orderBy.Length > 0) orderBy = @" ORDER BY " + orderBy;

            string sql = @"SELECT " + selectFieldsList + tablesList + where + orderBy;
            if (limit > 0) sql += @" LIMIT " + limit;
            if (offset > 0) sql += @" OFFSET " + offset;
            return sql;
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"'", @"''").Replace(@"%", @"%%");
        }

        public override Geometry ReadGeometry(object value)
        {
            byte[] geometryData = value as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, true);
            }
            return null;
        }

        public override string func_UTC_NOW
        {
            get { return @"UTC_TIMESTAMP()"; }
        }

        public override string type_AUTOINCREMENT { get { return @"AUTO_INCREMENT"; } }

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
    }
}
