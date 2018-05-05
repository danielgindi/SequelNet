using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using dg.Sql.Sql.Spatial;
using System.Globalization;

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
        
        #endregion

        #region Executing

        public override int ExecuteNonQuery(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            using (var command = new SqlCommand(querySql, (SqlConnection)Connection, Transaction as SqlTransaction))
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

            using (var command = new SqlCommand(querySql, (SqlConnection)Connection, Transaction as SqlTransaction))
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

            var command = new SqlCommand(querySql, (SqlConnection)Connection, Transaction as SqlTransaction);
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

            var command = new SqlCommand(querySql, (SqlConnection)Connection, Transaction as SqlTransaction);
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
            return new DataReaderBase(((SqlCommand)command).ExecuteReader(), attachConnectionToReader ? this : null);
        }

        public override DataSet ExecuteDataSet(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            using (var command = new SqlCommand(querySql, (SqlConnection)Connection, Transaction as SqlTransaction))
            {
                DataSet dataSet = new DataSet();
                using (var adapter = new SqlDataAdapter(command))
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

            using (var adapter = new SqlDataAdapter((SqlCommand)command))
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

        static private Dictionary<string, MsSqlVersion> _Map_ConnStr_Version = new Dictionary<string, MsSqlVersion>();

        private MsSqlVersion _Version = null;

        public MsSqlVersion GetVersionData()
        {
            if (_Version == null)
            {
                MsSqlVersion version;
                if (_Map_ConnStr_Version.TryGetValue(Connection.ConnectionString, out version))
                {
                    _Version = version;
                }
                else
                {
                    try
                    {
                        version = new MsSqlVersion();
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
                        _Map_ConnStr_Version[Connection.ConnectionString] = _Version;
                    }
                    catch { }
                }
            }
            return _Version;
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
                WrapFieldName(TableName), Enabled ? @"ON" : @"OFF");
            ExecuteNonQuery(sql);
        }

        public override bool CheckIfTableExists(string TableName)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            return ExecuteScalar(@"SELECT name FROM sysObjects WHERE name like " + PrepareValue(TableName)) != null;
        }

        #endregion

        #region Preparing values for SQL

        public override string WrapFieldName(string fieldName)
        { // Note: For performance, ignoring enclosed [] signs
            return '[' + fieldName + ']';
        }

        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }
        public override string PrepareValue(string value)
        {
            return @"N'" + EscapeString(value) + '\'';
        }
        public override string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%").Replace(@"_", @"\_");
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
            get { return 4000; }
        }

        public override string varchar_MAX
        {
            get { return "MAX"; }
        }

        public override string func_UTC_NOW()
        {
            return @"GETUTCDATE()";
        }

        public override string func_HOUR(string date)
        {
            return @"DATEPART(hour, " + date + ")";
        }

        public override string func_MINUTE(string date)
        {
            return @"DATEPART(minute, " + date + ")";
        }

        public override string func_SECOND(string date)
        {
            return @"DATEPART(second, " + date + ")";
        }

        public override string func_MD5_Hex(string value)
        {
            if (GetVersionData().MajorVersion < 10)
            {
                return @"SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5', " + value + ")), 3, 32)";
            }
            else
            {
                return @"CONVERT(VARCHAR(32), HASHBYTES('MD5', " + value + "), 2)";
            }
        }

        public override string func_SHA1_Hex(string value)
        {
            if (GetVersionData().MajorVersion < 10)
            {
                return @"SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('SHA1', " + value + ")), 3, 32)";
            }
            else
            {
                return @"CONVERT(VARCHAR(32), HASHBYTES('SHA1', " + value + "), 2)";
            }
        }

        public override string func_MD5_Binary(string value)
        {
            return @"HASHBYTES('MD5', " + value + ")";
        }

        public override string func_SHA1_Binary(string value)
        {
            return @"HASHBYTES('SHA1', " + value + ")";
        }

        public override string func_LENGTH(string value)
        {
            return @"LEN(" + value + ")";
        }

        public override string type_TINYINT { get { return @"TINYINT"; } }
        public override string type_UNSIGNEDTINYINT { get { return @"TINYINT"; } }
        public override string type_SMALLINT { get { return @"SMALLINT"; } }
        public override string type_UNSIGNEDSMALLINT { get { return @"SMALLINT"; } }
        public override string type_INT { get { return @"INT"; } }
        public override string type_UNSIGNEDINT { get { return @"INT"; } }
        public override string type_BIGINT { get { return @"BIGINT"; } }
        public override string type_UNSIGNEDBIGINT { get { return @"BIGINT"; } }
        public override string type_NUMERIC { get { return @"NUMERIC"; } }
        public override string type_DECIMAL { get { return @"DECIMAL"; } }
        public override string type_MONEY { get { return @"MONEY"; } }
        public override string type_FLOAT { get { return @"FLOAT(4)"; } }
        public override string type_DOUBLE { get { return @"FLOAT(8)"; } }
        public override string type_VARCHAR { get { return @"NVARCHAR"; } }
        public override string type_CHAR { get { return @"NCHAR"; } }
        public override string type_TEXT { get { return @"NTEXT"; } }
        public override string type_MEDIUMTEXT { get { return @"NTEXT"; } }
        public override string type_LONGTEXT { get { return @"NTEXT"; } }
        public override string type_BOOLEAN { get { return @"bit"; } }
        public override string type_DATETIME { get { return @"DATETIME"; } }
        public override string type_BLOB { get { return @"IMAGE"; } }
        public override string type_GUID { get { return @"UNIQUEIDENTIFIER"; } }
        public override string type_AUTOINCREMENT { get { return @"IDENTITY"; } }
        public override string type_AUTOINCREMENT_BIGINT { get { return @"IDENTITY"; } }
        public override string type_JSON { get { return @"TEXT"; } }
        public override string type_JSON_BINARY { get { return @"TEXT"; } }

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
