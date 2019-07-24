using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

// This class needs a little cleanup. But it's gonna break dependent code, so I'm gonna postpone it. In the meanwhile we will still use this pretty ugly code.

namespace dg.Sql.Connector
{
    public abstract class ConnectorBase : IDisposable
    {
        public enum SqlServiceType
        {
            UNKNOWN = 0,
            MYSQL = 1,
            MSSQL = 2,
            MSACCESS = 3,
            POSTGRESQL = 4
        }

        #region Instancing

        private DbConnection _Connection = null;

        static Type s_ConnectorType = null;
        static public ConnectorBase NewInstance()
        {
            if (s_ConnectorType == null)
            {
                s_ConnectorType = FindConnectorType();
            }
            return (ConnectorBase)System.Activator.CreateInstance(s_ConnectorType);
        }

        static public ConnectorBase NewInstance(string connectionStringKey)
        {
            if (s_ConnectorType == null)
            {
                s_ConnectorType = FindConnectorType();
            }
            return (ConnectorBase)System.Activator.CreateInstance(s_ConnectorType, new string[] { connectionStringKey });
        }

        virtual public SqlServiceType TYPE
        {
            get { return SqlServiceType.UNKNOWN; }
        }

        static private Type FindConnectorType()
        {
            Type type = null;
            try
            {
                string connector = ConfigurationManager.AppSettings[@"dg.Sql.Connector"];
                if (!string.IsNullOrEmpty(connector))
                {
                    Assembly asm = Assembly.Load(@"dg.Sql.Connector." + connector);
                    type = asm.GetType(@"dg.Sql.Connector." + connector + @"Connector");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(@"dg.ConnectorBase.FindConnectorType error: {0}", ex));
            }
            if (type == null) System.Web.Compilation.BuildManager.GetType(@"dg.Sql.Connector.MySqlConnector", false);
            return type;
        }

        public static string FindConnectionString(string ConnectionStringKey)
        {
            ConnectionStringSettings connString = null;

            if (ConnectionStringKey != null)
            {
                connString = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringKey];
                if (connString != null) return connString.ConnectionString;
            }

            string appConnString = ConfigurationManager.AppSettings[@"dg.Sql::ConnectionStringKey"];
            if (appConnString != null && appConnString.Length > 0) connString = System.Configuration.ConfigurationManager.ConnectionStrings[appConnString];
            if (connString != null) return connString.ConnectionString;

            appConnString = ConfigurationManager.AppSettings[@"dg.Sql::ConnectionString"];
            if (appConnString != null && appConnString.Length > 0) return appConnString;

            connString = System.Configuration.ConfigurationManager.ConnectionStrings[@"dg.Sql"];
            if (connString != null) return connString.ConnectionString;

            return System.Configuration.ConfigurationManager.ConnectionStrings[0].ConnectionString;
        }

        public static string FindConnectionString()
        {
            return FindConnectionString(null);
        }

        abstract public FactoryBase Factory
        {
            get;
        }

        public DbConnection Connection
        {
            get { return _Connection; }
            protected set { _Connection = value; }
        }

        #endregion

        #region IDisposable

        public void Dispose()
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

        public virtual void Close()
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

        #endregion

        #region Executing
        
        public virtual int ExecuteNonQuery(DbCommand command)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;
            return command.ExecuteNonQuery();
        }
        
        public virtual object ExecuteScalar(DbCommand command)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;
            return command.ExecuteScalar();
        }

        public virtual DataReaderBase ExecuteReader(DbCommand command, bool attachCommandToReader = false, bool attachConnectionToReader = false)
        {
            try
            {
                if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

                command.Connection = Connection;
                command.Transaction = Transaction;

                return new DataReaderBase(
                    command.ExecuteReader(),
                    attachCommandToReader ? command : null,
                    attachConnectionToReader ? this : null);
            }
            catch (Exception ex)
            {
                if (attachCommandToReader && command != null)
                    command.Dispose();

                if (attachConnectionToReader && Connection != null)
                    Connection.Dispose();

                throw ex;
            }
        }
        
        public virtual DataSet ExecuteDataSet(DbCommand command)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;

            var dataSet = new DataSet();

            using (var adapter = Factory.NewDataAdapter(command))
            {
                adapter.Fill(dataSet);
            }

            return dataSet;
        }

        public virtual int ExecuteNonQuery(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            using (var command = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public virtual object ExecuteScalar(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            using (var command = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return command.ExecuteScalar();
            }
        }

        public virtual DataReaderBase ExecuteReader(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            var command = Factory.NewCommand(querySql, Connection, Transaction);
            return ExecuteReader(command, true);
        }

        public virtual DataReaderBase ExecuteReader(string querySql, bool attachConnectionToReader)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            var command = Factory.NewCommand(querySql, Connection, Transaction);
            return ExecuteReader(command, true, attachConnectionToReader);
        }

        public virtual DataSet ExecuteDataSet(string querySql)
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();

            using (var cmd = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return ExecuteDataSet(cmd);
            }
        }

        public abstract int ExecuteScript(string querySql);

        #endregion

        #region Utilities

        virtual public string GetVersion()
        {
            throw new NotImplementedException(@"GetVersion not implemented in connector of type " + this.GetType().Name);
        }

        virtual public bool SupportsSelectPaging()
        {
            return false;
        }

        abstract public object GetLastInsertID();

        virtual public void SetIdentityInsert(string TableName, bool Enabled) { }

        abstract public bool CheckIfTableExists(string TableName);

        #endregion

        #region Transactions
        
        private DbTransaction _Transaction = null;
        private Stack<DbTransaction> _Transactions = null;

        public virtual bool BeginTransaction()
        {
            try
            {
                if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction();
                if (_Transactions == null) _Transactions = new Stack<DbTransaction>(1);
                _Transactions.Push(_Transaction);
                return (_Transaction != null);
            }
            catch (DbException) { }
            return false;
        }

        public virtual bool BeginTransaction(IsolationLevel IsolationLevel)
        {
            try
            {
                if (_Connection.State != System.Data.ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction(IsolationLevel);
                if (_Transactions == null) _Transactions = new Stack<DbTransaction>(1);
                _Transactions.Push(_Transaction);
            }
            catch (DbException) { return false; }
            return (_Transaction != null);
        }

        public virtual bool CommitTransaction()
        {
            if (_Transaction == null) return false;
            else
            {
                _Transactions.Pop();

                try
                {
                    _Transaction.Commit();
                }
                catch (DbException) { return false; }

                if (_Transactions.Count > 0) _Transaction = _Transactions.Peek();
                else _Transaction = null;
                return true;
            }
        }

        public virtual bool RollbackTransaction()
        {
            if (_Transaction == null) return false;
            else
            {
                _Transactions.Pop();

                try
                {
                    _Transaction.Rollback();
                }
                catch (DbException) { return false; }

                if (_Transactions.Count > 0) _Transaction = _Transactions.Peek();
                else _Transaction = null;
                return true;
            }
        }

        public virtual bool HasTransaction
        {
            get { return _Transaction != null; }
        }

        public virtual int CurrentTransactions
        {
            get { return _Transactions == null ? 0 : _Transactions.Count; }
        }

        public DbTransaction Transaction
        {
            get { return _Transaction; }
        }

        #endregion

        #region Preparing values for SQL

        abstract public string WrapFieldName(string fieldName);

        [Obsolete]
        public string EncloseFieldName(string fieldName)
        {
            return WrapFieldName(fieldName);
        }

        public virtual string EscapeString(string value)
        {
            return value.Replace(@"'", @"''");
        }

        public virtual string PrepareValue(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public virtual string PrepareValue(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public virtual string PrepareValue(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public virtual string PrepareValue(bool value)
        {
            return value ? @"1" : @"0";
        }

        abstract public string PrepareValue(Guid value);

        public virtual string PrepareValue(string value)
        {
            if (value == null) return @"NULL";
            else return '\'' + EscapeString(value) + '\'';
        }

        public virtual string PrepareValue(object value, Query relatedQuery = null)
        {
            if (value == null || value is DBNull) return @"NULL";
            else if (value is string)
            {
                return PrepareValue((string)value);
            }
            else if (value is DateTime)
            {
                return '\'' + FormatDate((DateTime)value) + '\'';
            }
            else if (value is Guid)
            {
                return PrepareValue((Guid)value);
            }
            else if (value is bool)
            {
                return PrepareValue((bool)value);
            }
            else if (value is decimal)
            { // Must be formatted specifically, to avoid decimal separator confusion
                return PrepareValue((decimal)value);
            }
            else if (value is float)
            {
             // Must be formatted specifically, to avoid decimal separator confusion
                return PrepareValue((float)value);
            }
            else if (value is double)
            { // Must be formatted specifically, to avoid decimal separator confusion
                return PrepareValue((double)value);
            }
            else if (value is IPhrase)
            {
                return ((IPhrase)value).BuildPhrase(this, relatedQuery);
            }
            else if (value is Geometry)
            {
                StringBuilder sb = new StringBuilder();
                ((Geometry)value).BuildValue(sb, this);
                return sb.ToString();
            }
            else if (value is Where)
            {
                StringBuilder sb = new StringBuilder();
                ((Where)value).BuildCommand(sb, true, new Where.BuildContext
                {
                    Conn = this,
                    RelatedQuery = relatedQuery
                });
                return sb.ToString();
            }
            else if (value is WhereList)
            {
                StringBuilder sb = new StringBuilder();
                ((WhereList)value).BuildCommand(sb, new Where.BuildContext
                {
                    Conn = this,
                    RelatedQuery = relatedQuery
                });
                return sb.ToString();
            }
            else if (value.GetType().BaseType.Name == @"Enum")
            {
                var underlyingValue = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
                if (underlyingValue is string || underlyingValue is char)
                {
                    return PrepareValue(underlyingValue.ToString());
                }

                return underlyingValue.ToString();
            }
            else return value.ToString();
        }

        abstract public string FormatDate(DateTime dateTime);

        public virtual string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%");
        }

        public virtual string LikeEscapingStatement
        {
            get { return @"ESCAPE('\')"; }
        }

        #endregion

        #region Reading values from SQL

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column name.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="IndexOutOfRangeException">No column with the specified name was found</exception>
        public virtual Geometry ReadGeometry(object value)
        {
            throw new NotImplementedException(@"ReadGeometry not implemented for this connector");
        }

        #endregion

        #region Engine-specific keywords

        public virtual int varchar_MAX_VALUE
        {
            get { return 255; }
        }

        public virtual string varchar_MAX
        {
            get { return null; } // Not supported
        }

        public virtual string func_UTC_NOW()
        {
            return @"NOW()";
        }

        public virtual string func_LOWER(string value)
        {
            return @"LOWER(" + value + ")";
        }

        public virtual string func_UPPER(string value)
        {
            return @"UPPER(" + value + ")";
        }

        public virtual string func_LENGTH(string value)
        {
            return @"LENGTH(" + value + ")";
        }

        public virtual string func_YEAR(string date)
        {
            return @"YEAR(" + date + ")";
        }

        public virtual string func_MONTH(string date)
        {
            return @"MONTH(" + date + ")";
        }

        public virtual string func_DAY(string date)
        {
            return @"DAY(" + date + ")";
        }

        public virtual string func_HOUR(string date)
        {
            return @"HOUR(" + date + ")";
        }

        public virtual string func_MINUTE(string date)
        {
            return @"MINUTE(" + date + ")";
        }

        public virtual string func_SECOND(string date)
        {
            return @"SECONDS(" + date + ")";
        }

        public virtual string func_MD5_Hex(string value)
        {
            return @"MD5(" + value + ")";
        }

        public virtual string func_SHA1_Hex(string value)
        {
            return @"SHA1(" + value + ")";
        }

        public virtual string func_MD5_Binary(string value)
        {
            return @"UNHEX(MD5(" + value + "))";
        }

        public virtual string func_SHA1_Binary(string value)
        {
            return @"UNHEX(SHA1(" + value + "))";
        }

        public virtual string func_ST_X(string pt)
        {
            return "ST_X(" + pt + ")";
        }

        public virtual string func_ST_Y(string pt)
        {
            return "ST_Y(" + pt + ")";
        }

        public virtual string func_ST_Contains(string g1, string g2)
        {
            return "ST_Contains(" + g1 + ", " + g2 + ")";
        }

        public virtual string func_ST_GeomFromText(string text, string srid = null)
        {
            return "ST_GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public virtual string func_ST_GeogFromText(string text, string srid = null)
        {
            return "ST_GeogFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public virtual void BuildNullSafeEqualsTo(
            Where where,
            bool negate,
            StringBuilder outputBuilder,
            Where.BuildContext context)
        {
            var wl = new WhereList();

            wl.Add(new Where
            {
                Condition = WhereCondition.OR,
                FirstTableName = where.FirstTableName,
                First = where.First,
                FirstType = where.FirstType,
                Comparison = negate ? WhereComparison.NotEqualsTo : WhereComparison.EqualsTo,
                SecondTableName = where.SecondTableName,
                Second = where.Second,
                SecondType = where.SecondType,
            });

            wl.Add(new Where(WhereCondition.OR,
                    new Where
                    {
                        FirstTableName = where.FirstTableName,
                        First = where.First,
                        FirstType = where.FirstType,
                        Comparison = WhereComparison.Is,
                        Second = null,
                        SecondType = ValueObjectType.Value,
                    },
                    ValueObjectType.Value,
                    negate ? WhereComparison.NotEqualsTo : WhereComparison.EqualsTo,
                    new Where
                    {
                        FirstTableName = where.SecondTableName,
                        First = where.Second,
                        FirstType = where.SecondType,
                        Comparison = WhereComparison.Is,
                        Second = null,
                        SecondType = ValueObjectType.Value,
                    },
                    ValueObjectType.Value
                ));

            wl.BuildCommand(outputBuilder, context);
        }

        public virtual string type_AUTOINCREMENT { get { return @"AUTOINCREMENT"; } }
        public virtual string type_AUTOINCREMENT_BIGINT { get { return @"AUTOINCREMENT"; } }

        public virtual string type_TINYINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDTINYINT { get { return @"INT"; } }
        public virtual string type_SMALLINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDSMALLINT { get { return @"INT"; } }
        public virtual string type_INT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDINT { get { return @"INT"; } }
        public virtual string type_BIGINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDBIGINT { get { return @"INT"; } }
        public virtual string type_NUMERIC { get { return @"NUMERIC"; } }
        public virtual string type_DECIMAL { get { return @"DECIMAL"; } }
        public virtual string type_MONEY { get { return @"DECIMAL"; } }
        public virtual string type_FLOAT { get { return @"FLOAT"; } }
        public virtual string type_DOUBLE { get { return @"DOUBLE"; } }
        public virtual string type_VARCHAR { get { return @"NVARCHAR"; } }
        public virtual string type_CHAR { get { return @"NCHAR"; } }
        public virtual string type_TEXT { get { return @"NTEXT"; } }
        public virtual string type_MEDIUMTEXT { get { return @"NTEXT"; } }
        public virtual string type_LONGTEXT { get { return @"NTEXT"; } }
        public virtual string type_BOOLEAN { get { return @"BOOLEAN"; } }
        public virtual string type_DATETIME { get { return @"DATETIME"; } }
        public virtual string type_BLOB { get { return @"BLOB"; } }
        public virtual string type_GUID { get { return @"GUID"; } }
        public virtual string type_JSON { get { return @"TEXT"; } }
        public virtual string type_JSON_BINARY { get { return @"TEXT"; } }

        public virtual string type_GEOMETRY { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOMETRYCOLLECTION { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_POINT { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_LINESTRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_POLYGON { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_LINE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_CURVE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_SURFACE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_LINEARRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTIPOINT { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTILINESTRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTIPOLYGON { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTICURVE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTISURFACE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }

        public virtual string type_GEOGRAPHIC { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHICCOLLECTION { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_POINT { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_LINESTRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_POLYGON { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_LINE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_CURVE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_SURFACE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_LINEARRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTIPOINT { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTILINESTRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTIPOLYGON { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTICURVE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTISURFACE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        
        #endregion

        #region DB Mutex

        public enum SqlMutexOwner
        {
            /// <summary>
            /// Used to declare the mutex owned by the session, and expires when the session ends.
            /// </summary>
            Session,

            /// <summary>
            /// Used to declare the mutex owned by the transaction, and expires when the transaction ends.
            /// If <value>Transaction</value> is specified, then the lock must be acquired within a transaction.
            /// 
            /// * Not supported by MySql
            /// </summary>
            Transaction
        }

        /// <summary>
        /// Creates a mutex on the DB server.
        /// </summary>
        /// <param name="lockName">Unique name for the lock</param>
        /// <param name="owner">The owner of the lock. Partial support in different db servers.</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="dbPrincipal">The user that has permissions to the lock.</param>
        /// <returns><value>true</value> if the lock was acquired, <value>false</value> if failed or timed out.</returns>
        public virtual bool GetLock(string lockName, TimeSpan timeout, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            throw new NotImplementedException(@"GetLock");
        }

        /// <summary>
        /// Releases a mutex on the DB server.
        /// </summary>
        /// <param name="lockName">Unique name for the lock</param>
        /// <param name="owner">The owner of the lock. Partial support in different db servers.</param>
        /// <param name="dbPrincipal">The user that has permissions to the lock.</param>
        /// <returns><value>true</value> if the lock was release, <value>false</value> if failed, not exists or timed out.</returns>
        public virtual bool ReleaseLock(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            throw new NotImplementedException(@"ReleaseLock");
        }

        #endregion
    }
}
