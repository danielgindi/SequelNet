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

        #endregion

        #region Executing

        public override int ExecuteNonQuery(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            using (var command = new MySqlCommand(querySql, (MySqlConnection)Connection, Transaction as MySqlTransaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public override int ExecuteNonQuery(DbCommand command)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;
            return command.ExecuteNonQuery();
        }

        public override object ExecuteScalar(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            using (var command = new MySqlCommand(querySql, (MySqlConnection)Connection, Transaction as MySqlTransaction))
            {
                return command.ExecuteScalar();
            }
        }

        public override object ExecuteScalar(DbCommand command)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;
            return command.ExecuteScalar();
        }

        public override DataReaderBase ExecuteReader(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            var command = new MySqlCommand(querySql, (MySqlConnection)Connection, Transaction as MySqlTransaction);
            try
            {
                return new DataReaderBase(command.ExecuteReader(), command);
            }
            catch (Exception ex)
            {
                command.Dispose();
                throw ex;
            }
        }

        public override DataReaderBase ExecuteReader(string querySql, bool attachConnectionToReader)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            var command = new MySqlCommand(querySql, (MySqlConnection)Connection, Transaction as MySqlTransaction);
            try
            {
                return new DataReaderBase(command.ExecuteReader(), command, attachConnectionToReader ? this : null);
            }
            catch (Exception ex)
            {
                command.Dispose();
                throw ex;
            }
        }

        public override DataReaderBase ExecuteReader(DbCommand command, bool attachCommandToReader = false, bool attachConnectionToReader = false)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            command.Connection = Connection;
            command.Transaction = Transaction;

            return new DataReaderBase(
                command.ExecuteReader(),
                attachCommandToReader ? command : null,
                attachConnectionToReader ? this : null);
        }

        public override DataReaderBase ExecuteReader(DbCommand command, bool attachConnectionToReader)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;
            return new DataReaderBase(((MySqlCommand)command).ExecuteReader(), attachConnectionToReader ? this : null);
        }

        public override DataSet ExecuteDataSet(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            using (var cmd = new MySqlCommand(querySql, (MySqlConnection)Connection, Transaction as MySqlTransaction))
            {
                DataSet dataSet = new DataSet();
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dataSet);
                }
                return dataSet;
            }
        }

        public override DataSet ExecuteDataSet(DbCommand command)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;
            DataSet dataSet = new DataSet();

            using (var adapter = new MySqlDataAdapter((MySqlCommand)command))
            {
                adapter.Fill(dataSet);
            }

            return dataSet;
        }

        public override int ExecuteScript(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            MySqlScript script = new MySqlScript((MySqlConnection)Connection, querySql);
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
                if (_Map_ConnStr_SqlMode.TryGetValue(Connection.ConnectionString, out sqlMode))
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
                    _Map_ConnStr_SqlMode[Connection.ConnectionString] = sqlMode;
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
                if (_Map_ConnStr_Version.TryGetValue(Connection.ConnectionString, out version))
                {
                    _Version = version;
                }
                else
                {
                    try
                    {
                        version = ExecuteScalar("SELECT @@VERSION").ToString();
                        _Version = version;
                        _Map_ConnStr_Version[Connection.ConnectionString] = _Version;
                    }
                    catch { }
                }
            }
            return _Version;
        }

        bool? _Is5_0_3OrLater = null;
        private bool Is5_0_3OrLater()
        {
            if (_Is5_0_3OrLater == null)
            {
                _Is5_0_3OrLater = GetVersion().CompareTo("5.0.3") >= 0;
            }
            return _Is5_0_3OrLater.Value;
        }

        bool? _Is5_7OrLater = null;
        private bool Is5_7OrLater()
        {
            if (_Is5_7OrLater == null)
            {
                _Is5_7OrLater = GetVersion().CompareTo("5.7") >= 0;
            }
            return _Is5_7OrLater.Value;
        }

        bool? _Is8_0OrLater = null;
        private bool Is8_0OrLater()
        {
            if (_Is8_0OrLater == null)
            {
                _Is8_0OrLater = GetVersion().CompareTo("8.0") >= 0;
            }
            return _Is8_0OrLater.Value;
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

        public override void SetIdentityInsert(string TableName, bool Enabled)
        {
            // Nothing to do. In MySql IDENTITY_INSERT is always allowed
        }

        public override bool CheckIfTableExists(string TableName)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            return ExecuteScalar(@"SHOW TABLES LIKE " + PrepareValue(TableName)) != null;
        }

        #endregion

        #region Preparing values for SQL

        public override string WrapFieldName(string fieldName)
        {
            return '`' + fieldName.Replace("`", "``") + '`';
        }


        private static string CharactersNeedsBackslashes = // Other special characters for escaping
            "\u005c\u00a5\u0160\u20a9\u2216\ufe68\uff3c";
        private static string CharactersNeedsDoubling = // Kinds of quotes...
            "\u0027\u0060\u00b4\u02b9\u02ba\u02bb\u02bc\u02c8\u02ca\u02cb\u02d9\u0300\u0301\u2018\u2019\u201a\u2032\u2035\u275b\u275c\uff07";

        public static string EscapeStringWithBackslashes(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (CharactersNeedsDoubling.IndexOf(c) >= 0 || CharactersNeedsBackslashes.IndexOf(c) >= 0)
                {
                    sb.Append("\\");
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string EscapeStringWithoutBackslashes(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
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

        public override string EscapeString(string value)
        {
            if (GetMySqlMode().NoBackSlashes)
            {
                return EscapeStringWithoutBackslashes(value);
            }
            else
            {
                return EscapeStringWithBackslashes(value);
            }
        }

        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }

        public override string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"'", @"''").Replace(@"%", @"%%");
        }

        public override string LikeEscapingStatement
        {
            get { return @"ESCAPE('\\')"; }
        }

        #endregion

        #region Reading values from SQL

        public override Geometry ReadGeometry(object value)
        {
            byte[] geometryData = value as byte[];
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
                if (Is5_0_3OrLater())
                {
                    return 21845;
                }
                else
                {
                    return 255;
                }
            }
        }

        public override string func_UTC_NOW()
        {
            return @"UTC_TIMESTAMP()";
        }

        public override string func_ST_X(string pt)
        {
            if (Is5_7OrLater())
            {
                return "ST_X(" + pt + ")";
            }
            else
            {
                return "X(" + pt + ")";
            }
        }

        public override string func_ST_Y(string pt)
        {
            if (Is5_7OrLater())
            {
                return "ST_Y(" + pt + ")";
            }
            else
            {
                return "Y(" + pt + ")";
            }
        }

        public override string func_ST_Contains(string g1, string g2)
        {
            if (Is5_7OrLater())
            {
                return "ST_Contains(" + g1 + ", " + g2 + ")";
            }
            else
            {
                return "MBRContains(" + g1 + ", " + g2 + ")";
            }
        }

        public override string func_ST_GeomFromText(string text, string srid = null)
        {
            if (Is5_7OrLater())
            {
                return "ST_GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
            }
            else
            {
                return "GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
            }
        }

        public override string func_ST_GeogFromText(string text, string srid = null)
        {
            return func_ST_GeomFromText(text, srid);
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
        public override string type_JSON { get { return @"JSON"; } }
        public override string type_JSON_BINARY { get { return @"JSON"; } }

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

        public override bool GetLock(string lockName, TimeSpan timeout, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
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

        public override bool ReleaseLock(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
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
