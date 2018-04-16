using System;
using System.Collections.Generic;
using System.Web;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Text;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
{
    public class PostgreSQLConnector : ConnectorBase
    {
        #region Instancing

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.POSTGRESQL; }
        }

        public static NpgsqlConnection CreateSqlConnection(string connectionStringKey)
        {
            return new NpgsqlConnection(FindConnectionString(connectionStringKey));
        }

        private NpgsqlConnection _Connection = null;

        public PostgreSQLConnector()
        {
            _Connection = CreateSqlConnection(null);
        }
        public PostgreSQLConnector(string connectionStringKey)
        {
            _Connection = CreateSqlConnection(connectionStringKey);
        }
        ~PostgreSQLConnector()
        {
            Dispose(false);
        }

        public override void Close()
        {
            try
            {
                if (_Connection != null && _Connection.State != ConnectionState.Closed)
                {
                    try
                    {
                        while (HasTransaction)
                            RollbackTransaction();
                    }
                    catch { /*ignore errors here*/ }

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

        public override int ExecuteNonQuery(string querySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(querySql, _Connection, _Transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public override int ExecuteNonQuery(DbCommand command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            command.Connection = _Connection;
            command.Transaction = _Transaction;
            return command.ExecuteNonQuery();
        }

        public override object ExecuteScalar(string querySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(querySql, _Connection, _Transaction))
            {
                return command.ExecuteScalar();
            }
        }

        public override object ExecuteScalar(DbCommand command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            command.Connection = _Connection;
            command.Transaction = _Transaction;
            return command.ExecuteScalar();
        }

        public override DataReaderBase ExecuteReader(string querySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(querySql, _Connection, _Transaction))
            {
                return new DataReaderBase(command.ExecuteReader());
            }
        }

        public override DataReaderBase ExecuteReader(string querySql, bool attachConnectionToReader)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(querySql, _Connection, _Transaction))
            {
                return new DataReaderBase(command.ExecuteReader(), attachConnectionToReader ? this : null);
            }
        }

        public override DataReaderBase ExecuteReader(DbCommand command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            command.Connection = _Connection;
            command.Transaction = _Transaction;
            return new DataReaderBase(((NpgsqlCommand)command).ExecuteReader());
        }

        public override DataReaderBase ExecuteReader(DbCommand command, bool attachConnectionToReader)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            command.Connection = _Connection;
            command.Transaction = _Transaction;
            return new DataReaderBase(((NpgsqlCommand)command).ExecuteReader(), attachConnectionToReader ? this : null);
        }

        public override DataSet ExecuteDataSet(string querySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (NpgsqlCommand cmd = new NpgsqlCommand(querySql, _Connection, _Transaction))
            {
                DataSet dataSet = new DataSet();
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dataSet);
                }
                return dataSet;
            }
        }

        public override DataSet ExecuteDataSet(DbCommand command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            command.Connection = _Connection;
            command.Transaction = _Transaction;
            DataSet dataSet = new DataSet();
            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter((NpgsqlCommand)command))
            {
                adapter.Fill(dataSet);
            }
            return dataSet;
        }

        public override int ExecuteScript(string querySql)
        {
            throw new NotImplementedException(@"ExecuteScript");
        }

        #endregion

        #region Utilities

        static private Dictionary<string, PostgreSQLMode> _Map_ConnStr_SqlMode = new Dictionary<string, PostgreSQLMode>();
        static private Dictionary<string, string> _Map_ConnStr_Version = new Dictionary<string, string>();

        private PostgreSQLMode _PostgreSQLMode = null;
        private string _Version = null;

        public PostgreSQLMode GetPostgreSQLMode()
        {
            if (_PostgreSQLMode == null)
            {
                PostgreSQLMode sqlMode;
                if (_Map_ConnStr_SqlMode.TryGetValue(_Connection.ConnectionString, out sqlMode))
                {
                    _PostgreSQLMode = sqlMode;
                }
                else
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
                    _Map_ConnStr_SqlMode[_Connection.ConnectionString] = sqlMode;
                    _PostgreSQLMode = sqlMode;
                }
            }
            return _PostgreSQLMode;
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
                        version = ExecuteScalar("select version()").ToString();
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

        public NpgsqlConnection GetUnderlyingConnection()
        {
            return _Connection;
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"select lastval() AS id");
        }

        public override void SetIdentityInsert(string TableName, bool Enabled)
        {
            // Nothing to do. In PostgreSQL IDENTITY_INSERT is always allowed
        }

        public override bool CheckIfTableExists(string TableName)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            return ExecuteScalar(@"select * from information_schema.tables where table_name= " + PrepareValue(TableName)) != null;
        }

        #endregion

        #region Transactions

        NpgsqlTransaction _Transaction = null;
        Stack<NpgsqlTransaction> _Transactions = null;

        public override bool BeginTransaction()
        {
            try
            {
                if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction();
                if (_Transactions == null) _Transactions = new Stack<NpgsqlTransaction>(1);
                _Transactions.Push(_Transaction);
                return (_Transaction != null);
            }
            catch (NpgsqlException) { }
            return false;
        }

        public override bool BeginTransaction(IsolationLevel IsolationLevel)
        {
            try
            {
                if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction(IsolationLevel);
                if (_Transactions == null) _Transactions = new Stack<NpgsqlTransaction>(1);
                _Transactions.Push(_Transaction);
            }
            catch (NpgsqlException) { return false; }
            return (_Transaction != null);
        }

        public override bool CommitTransaction()
        {
            if (_Transaction == null) return false;
            else
            {
                _Transactions.Pop();

                try
                {
                    _Transaction.Commit();
                }
                catch (NpgsqlException) { return false; }

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
                _Transactions.Pop();

                try
                {
                    _Transaction.Rollback();
                }
                catch (NpgsqlException) { return false; }

                if (_Transactions.Count > 0) _Transaction = _Transactions.Peek();
                else _Transaction = null;
                return true;
            }
        }

        public override bool HasTransaction
        {
            get { return _Transaction != null; }
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

        public override string WrapFieldName(string fieldName)
        { // Note: For performance, ignoring enclosed " signs
            return '"' + fieldName + '"';
        }

        public static string EscapeStringWithBackslashes(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c == '\'' || c == '\\')
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
                if (c == '\'')
                {
                    sb.Append(c);
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public override string EscapeString(string value)
        {
            if (GetPostgreSQLMode().StandardConformingStrings)
            {
                return EscapeStringWithoutBackslashes(value);
            }
            else
            {
                return EscapeStringWithBackslashes(value);
            }
        }

        public override string PrepareValue(bool value)
        {
            return value ? @"TRUE" : @"FALSE";
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
            return expression.Replace(@"_", @"\_").Replace(@"%", @"\%");
        }

        public override string LikeEscapingStatement
        {
            get 
            {
                if (GetPostgreSQLMode().StandardConformingStrings)
                {
                    return @"ESCAPE('\')";
                }
                else
                {
                    return @"ESCAPE('\\')";
                }
            }
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

        #region Engine-specific keywords

        public override int varchar_MAX_VALUE
        {
            get { return 357913937; }
        }

        public override string func_UTC_NOW()
        {
            return @"now() at time zone 'utc'";
        }

        public override string func_YEAR(string date)
        {
            return @"EXTRACT(YEAR FROM " + date + @")";
        }

        public override string func_MONTH(string date)
        {
            return @"EXTRACT(MONTH FROM " + date + @")";
        }

        public override string func_DAY(string date)
        {
            return @"EXTRACT(DAY FROM " + date + @")";
        }

        public override string func_HOUR(string date)
        {
            return @"EXTRACT(HOUR FROM " + date + @")";
        }

        public override string func_MINUTE(string date)
        {
            return @"EXTRACT(MINUTE FROM " + date + @")";
        }

        public override string func_SECOND(string date)
        {
            return @"EXTRACT(SECOND FROM " + date + @")";
        }

        public override string func_MD5_Hex(string value)
        {
            return @"md5(" + value + ")";
        }

        public override string func_SHA1_Hex(string value)
        {
            throw new NotSupportedException("SHA1 is not supported by Postgresql");
        }

        public override string func_MD5_Binary(string value)
        {
            return @"decode(md5(" + value + "), 'hex')";
        }

        public override string func_SHA1_Binary(string value)
        {
            throw new NotSupportedException("SHA1 is not supported by Postgresql");
        }

        public override string type_AUTOINCREMENT { get { return @"SERIAL"; } } 
        public override string type_AUTOINCREMENT_BIGINT { get { return @"BIGSERIAL"; } }

        public override string type_TINYINT { get { return @"SMALLINT"; } }
        public override string type_UNSIGNEDTINYINT { get { return @"SMALLINT"; } }
        public override string type_SMALLINT { get { return @"SMALLINT"; } }
        public override string type_UNSIGNEDSMALLINT { get { return @"SMALLINT"; } }
        public override string type_INT { get { return @"INTEGER"; } }
        public override string type_UNSIGNEDINT { get { return @"INTEGER"; } }
        public override string type_BIGINT { get { return @"BIGINT"; } }
        public override string type_UNSIGNEDBIGINT { get { return @"BIGINT"; } }
        public override string type_NUMERIC { get { return @"NUMERIC"; } }
        public override string type_DECIMAL { get { return @"DECIMAL"; } }
        public override string type_MONEY { get { return @"DECIMAL"; } }
        public override string type_FLOAT { get { return @"FLOAT4"; } }
        public override string type_DOUBLE { get { return @"FLOAT8"; } }
        public override string type_VARCHAR { get { return @"VARCHAR"; } }
        public override string type_CHAR { get { return @"CHAR"; } }
        public override string type_TEXT { get { return @"TEXT"; } }
        public override string type_MEDIUMTEXT { get { return @"TEXT"; } }
        public override string type_LONGTEXT { get { return @"TEXT"; } }
        public override string type_BOOLEAN { get { return @"BOOLEAN"; } }
        public override string type_DATETIME { get { return @"TIMESTAMP"; } }
        public override string type_BLOB { get { return @"BYTEA"; } }
        public override string type_GUID { get { return @"UUID"; } }
        public override string type_JSON { get { return @"JSON"; } }
        public override string type_JSON_BINARY { get { return @"JSONB"; } }

        public override string type_GEOMETRY { get { return @"GEOMETRY"; } }
        public override string type_GEOMETRYCOLLECTION { get { return @"GEOMETRY"; } }
        public override string type_POINT { get { return @"GEOMETRY"; } }
        public override string type_LINESTRING { get { return @"GEOMETRY"; } }
        public override string type_POLYGON { get { return @"GEOMETRY"; } }
        public override string type_LINE { get { return @"GEOMETRY"; } }
        public override string type_CURVE { get { return @"GEOMETRY"; } }
        public override string type_SURFACE { get { return @"GEOMETRY"; } }
        public override string type_LINEARRING { get { return @"GEOMETRY"; } }
        public override string type_MULTIPOINT { get { return @"GEOMETRY"; } }
        public override string type_MULTILINESTRING { get { return @"GEOMETRY"; } }
        public override string type_MULTIPOLYGON { get { return @"GEOMETRY"; } }
        public override string type_MULTICURVE { get { return @"GEOMETRY"; } }
        public override string type_MULTISURFACE { get { return @"GEOMETRY"; } }

        public override string type_GEOGRAPHIC { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHICCOLLECTION { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_POINT { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_LINESTRING { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_POLYGON { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_LINE { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_CURVE { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_SURFACE { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_LINEARRING { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTIPOINT { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTILINESTRING { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTIPOLYGON { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTICURVE { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTISURFACE { get { return @"GEOGRAPHIC"; } }

        #endregion
    }
}
