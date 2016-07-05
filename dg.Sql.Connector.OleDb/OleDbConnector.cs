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

        public override int ExecuteNonQuery(string querySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (OleDbCommand command = new OleDbCommand(querySql, _Connection, _Transaction))
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
            using (OleDbCommand command = new OleDbCommand(querySql, _Connection, _Transaction))
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
            using (OleDbCommand command = new OleDbCommand(querySql, _Connection, _Transaction))
            {
                return new DataReaderBase(command.ExecuteReader());
            }
        }

        public override DataReaderBase ExecuteReader(string querySql, bool attachConnectionToReader)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (OleDbCommand command = new OleDbCommand(querySql, _Connection, _Transaction))
            {
                return new DataReaderBase(command.ExecuteReader(), attachConnectionToReader ? this : null);
            }
        }

        public override DataReaderBase ExecuteReader(DbCommand command)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            command.Connection = _Connection;
            command.Transaction = _Transaction;
            return new DataReaderBase(((OleDbCommand)command).ExecuteReader());
        }

        public override DataReaderBase ExecuteReader(DbCommand command, bool attachConnectionToReader)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            command.Connection = _Connection;
            command.Transaction = _Transaction;
            return new DataReaderBase(((OleDbCommand)command).ExecuteReader(), attachConnectionToReader ? this : null);
        }

        public override DataSet ExecuteDataSet(string querySql)
        {
            if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
            using (OleDbCommand cmd = new OleDbCommand(querySql, _Connection, _Transaction))
            {
                DataSet dataSet = new DataSet();
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
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
            using (OleDbDataAdapter adapter = new OleDbDataAdapter((OleDbCommand)command))
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

        public override string WrapFieldName(string fieldName)
        { // Note: For performance, ignoring enclosed [] signs
            return '[' + fieldName + ']';
        }

        public override string EscapeString(string value)
        {
            return value.Replace(@"'", @"''");
        }

        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }

        public override string PrepareValue(bool value)
        {
            return value ? @"true" : @"false";
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

        public override string func_UTC_NOW()
        {
            return @"now()"; // NOT UTC
        }

        public override string func_LOWER(string value)
        {
            return @"LCASE(" + value + ")";
        }

        public override string func_UPPER(string value)
        {
            return @"UCASE(" + value + ")";
        }

        public override string func_LENGTH(string value)
        {
            return @"LEN(" + value + ")";
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
        public override string type_MONEY { get { return @"DECIMAL"; } }
        public override string type_FLOAT { get { return @"SINGLE"; } }
        public override string type_DOUBLE { get { return @"DOUBLE"; } }
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
