using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
{
    public class MsSqlConnector : ConnectorBase
    {
        SqlTransaction _transaction = null;
        Stack<SqlTransaction> _transactions = null;

        public override SqlServiceType TYPE
        {
            get { return SqlServiceType.MSSQL; }
        }

        public static SqlConnection CreateSqlConnection(string connectionStringKey)
        {
            return new SqlConnection(GetWebsiteConnectionString(connectionStringKey));
        }

        SqlConnection _conn = null;

        public MsSqlConnector()
        {
            _conn = CreateSqlConnection(null);
        }
        public MsSqlConnector(string connectionStringKey)
        {
            _conn = CreateSqlConnection(connectionStringKey);
        }
        ~MsSqlConnector()
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
            catch
            {

            }
            if (_conn != null) _conn.Dispose();
            _conn = null;
        }
        public SqlConnection GetConn()
        {
            return _conn;
        }
        public override int ExecuteNonQuery(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            using (SqlCommand command = new SqlCommand(strSQL, _conn, _transaction))
            {
                return command.ExecuteNonQuery();
            }
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
            using (SqlCommand command = new SqlCommand(strSQL, _conn, _transaction))
            {
                return command.ExecuteScalar();
            }
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
            using (SqlCommand command = new SqlCommand(strSQL, _conn, _transaction))
            {
                return new MsSqlDataReader(command.ExecuteReader());
            }
        }
        public override DataReaderBase ExecuteReader(String strSQL, bool attachConnectionToReader)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            using (SqlCommand command = new SqlCommand(strSQL, _conn, _transaction))
            {
                return new MsSqlDataReader(command.ExecuteReader(), attachConnectionToReader ? this : null);
            }
        }
        public override DataReaderBase ExecuteReader(DbCommand command)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return new MsSqlDataReader(((SqlCommand)command).ExecuteReader());
        }
        public override DataReaderBase ExecuteReader(DbCommand command, bool attachConnectionToReader)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            command.Connection = _conn;
            command.Transaction = _transaction;
            return new MsSqlDataReader(((SqlCommand)command).ExecuteReader(), attachConnectionToReader ? this : null);
        }
        public override DataSet ExecuteDataSet(String strSQL)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            using (SqlCommand command = new SqlCommand(strSQL, _conn, _transaction))
            {
                DataSet dataSet = new DataSet();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
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
            using (SqlDataAdapter adapter = new SqlDataAdapter((SqlCommand)command))
            {
                adapter.Fill(dataSet);
            }
            return dataSet;
        }
        public override int ExecuteScript(String strSQL)
        {
            throw new NotImplementedException(@"ExecuteScript");
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"SELECT @@identity AS id");
        }

        public override bool checkIfTableExists(string tableName)
        {
            if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
            return ExecuteScalar(@"SELECT name FROM sysObjects WHERE name like '" + fullEscape(tableName) + "'") != null;
        }

        public override bool beginTransaction()
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
                _transaction = _conn.BeginTransaction();
                if (_transactions == null) _transactions = new Stack<SqlTransaction>(1);
                _transactions.Push(_transaction);
                return (_transaction != null);
            }
            catch (SqlException) { }
            return false;
        }
        public override bool beginTransaction(IsolationLevel isolationLevel)
        {
            try
            {
                if (_conn.State != System.Data.ConnectionState.Open) _conn.Open();
                _transaction = _conn.BeginTransaction(isolationLevel);
                if (_transactions == null) _transactions = new Stack<SqlTransaction>(1);
                _transactions.Push(_transaction);
            }
            catch (SqlException) { return false; }
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
                catch (SqlException) { return false; }
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
                catch (SqlException) { return false; }
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

        public override void setIdentityInsert(string TableName, bool Enabled)
        {
            string sql = string.Format(@"SET IDENTITY_INSERT {0} {1}",
                encloseFieldName(TableName), Enabled ? @"ON" : @"OFF");
            ExecuteNonQuery(sql);
        }

        public override string fullEscape(string strToEscape)
        {
            return strToEscape.Replace(@"'", @"''");
        }
        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }
        public override string PrepareValue(string value)
        {
            return @"N'" + fullEscape(value) + '\'';
        }
        public override string encloseFieldName(string fieldName)
        {
            return '[' + fieldName + ']';
        }
        public override string formatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%").Replace(@"_", @"\_");
        }

        public override Geometry ReadGeometry(object value)
        {
            byte[] geometryData = value as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, false);
            }
            return null;
        }

        public override string func_UTC_NOW
        {
            get { return @"GETUTCDATE()"; }
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
    }
}
