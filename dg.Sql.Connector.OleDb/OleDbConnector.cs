using System;
using System.Collections.Generic;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
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

        private OleDbConnection _Connection = null;

        public OleDbConnector()
        {
            _Connection = CreateSqlConnection(null);
        }
        public OleDbConnector(string connectionStringKey)
        {
            _Connection = CreateSqlConnection(connectionStringKey);
        }
        ~OleDbConnector()
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
            using (OleDbCommand command = new OleDbCommand(QuerySql, _Connection, _Transaction))
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
            using (OleDbCommand command = new OleDbCommand(QuerySql, _Connection, _Transaction))
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
            using (OleDbCommand command = new OleDbCommand(QuerySql, _Connection, _Transaction))
            {
                return new OleDbDataReader(command.ExecuteReader());
            }
        }
        public override DataReaderBase ExecuteReader(string QuerySql, bool AttachConnectionToReader)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (OleDbCommand command = new OleDbCommand(QuerySql, _Connection, _Transaction))
            {
                return new OleDbDataReader(command.ExecuteReader(), AttachConnectionToReader ? this : null);
            }
        }
        public override DataReaderBase ExecuteReader(DbCommand Command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            Command.Connection = _Connection;
            Command.Transaction = _Transaction;
            return new OleDbDataReader(((OleDbCommand)Command).ExecuteReader());
        }
        public override DataReaderBase ExecuteReader(DbCommand Command, bool AttachConnectionToReader)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            Command.Connection = _Connection;
            Command.Transaction = _Transaction;
            return new OleDbDataReader(((OleDbCommand)Command).ExecuteReader(), AttachConnectionToReader ? this : null);
        }
        public override DataSet ExecuteDataSet(string QuerySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (OleDbCommand cmd = new OleDbCommand(QuerySql, _Connection, _Transaction))
            {
                DataSet dataSet = new DataSet();
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
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
            using (OleDbDataAdapter adapter = new OleDbDataAdapter((OleDbCommand)Command))
            {
                adapter.Fill(dataSet);
            }
            return dataSet;
        }
        public override int ExecuteScript(string QuerySql)
        {
            throw new NotImplementedException(@"ExecuteScript");
        }

        #endregion

        #region Utilities

        public override bool SupportsSelectPaging()
        {
            return false;
        }

        public OleDbConnection GetUnderlyingConnection()
        {
            return _Connection;
        }

        public override object GetLastInsertID()
        {
            return ExecuteScalar(@"SELECT @@identity AS id");
        }

        public override bool CheckIfTableExists(string TableName)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            return ExecuteScalar(@"SELECT name FROM MSysObjects WHERE name like '" + EscapeString(TableName) + "'") != null;
        }

        #endregion

        #region Transactions

        private OleDbTransaction _Transaction = null;
        private Stack<OleDbTransaction> _Transactions = null;

        public override bool BeginTransaction()
        {
            try
            {
                if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction();
                if (_Transactions == null) _Transactions = new Stack<OleDbTransaction>(1);
                _Transactions.Push(_Transaction);
                return (_Transaction != null);
            }
            catch (OleDbException) { }
            return false;
        }

        public override bool BeginTransaction(IsolationLevel IsolationLevel)
        {
            try
            {
                if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction(IsolationLevel);
                if (_Transactions == null) _Transactions = new Stack<OleDbTransaction>(1);
                _Transactions.Push(_Transaction);
            }
            catch (OleDbException) { return false; }
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
                catch (OleDbException) { return false; }
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
                catch (OleDbException) { return false; }
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
        { // Note: For performance, ignoring enclosed [] signs
            return '[' + FieldName + ']';
        }

        public override string EscapeString(string Value)
        {
            return Value.Replace(@"'", @"''");
        }

        public override string PrepareValue(Guid Value)
        {
            return '\'' + Value.ToString(@"D") + '\'';
        }

        public override string PrepareValue(bool Value)
        {
            return Value ? @"true" : @"false";
        }

        public override string FormatDate(DateTime DateTime)
        {
            return DateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string Expression)
        {
            return Expression.Replace(@"\", @"\\").Replace(@"%", @"\%").Replace(@"_", @"\_");
        }

        #endregion

        #region Reading values from SQL

        public override Geometry ReadGeometry(object Value)
        {
            byte[] geometryData = Value as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, false);
            }
            return null;
        }

        #endregion

        #region Engine-specific keywords

        public override string func_UTC_NOW
        {
            get { return @"now()"; } // NOT UTC
        }
        public override string func_LOWER
        {
            get { return @"LCASE"; }
        }
        public override string func_UPPER
        {
            get { return @"UCASE"; }
        }

        public override string func_HOUR(string Date)
        {
            return @"DATEPART(hour, " + Date + ")";
        }
        public override string func_MINUTE(string Date)
        {
            return @"DATEPART(minute, " + Date + ")";
        }
        public override string func_SECOND(string Date)
        {
            return @"DATEPART(second, " + Date + ")";
        }

        public override string type_TINYINT { get { return @"BYTE"; } }
        public override string type_UNSIGNEDTINYINT { get { return @"TINYINT"; } }
        public override string type_SMALLINT { get { return @"SHORT"; } }
        public override string type_UNSIGNEDSMALLINT { get { return @"SHORT"; } }
        public override string type_INT { get { return @"INT"; } }
        public override string type_UNSIGNEDINT { get { return @"INT"; } }
        public override string type_BIGINT { get { return @"INT"; } }
        public override string type_UNSIGNEDBIGINT { get { return @"INT"; } }
        public override string type_NUMERIC { get { return @"NUMERIC"; } }
        public override string type_DECIMAL { get { return @"DECIMAL"; } }
        public override string type_VARCHAR { get { return @"VARCHAR"; } }
        public override string type_CHAR { get { return @"CHAR"; } }
        public override string type_TEXT { get { return @"TEXT"; } }
        public override string type_MEDIUMTEXT { get { return @"TEXT"; } }
        public override string type_LONGTEXT { get { return @"TEXT"; } }
        public override string type_BOOLEAN { get { return @"BIT"; } }
        public override string type_DATETIME { get { return @"DATETIME"; } }
        public override string type_GUID { get { return @"UNIQUEIDENTIFIER"; } }
        public override string type_BLOB { get { return @"IMAGE"; } }
        public override string type_AUTOINCREMENT { get { return @"AUTOINCREMENT"; } }
        public override string type_AUTOINCREMENT_BIGINT { get { return @"AUTOINCREMENT"; } }

        #endregion
    }
}
