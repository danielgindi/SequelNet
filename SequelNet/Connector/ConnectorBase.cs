using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

[assembly: CLSCompliant(true)]

namespace SequelNet.Connector
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

        static ConcurrentDictionary<string, Type> s_ConnectorTypeMap = new ConcurrentDictionary<string, Type>();
        static Type s_DefaultConnectorType = null;
        static string s_DefaultConnectorTypeName;
        static string s_DefaultConnectionString;

        private static Type GetConnectorType(string typeName)
        {
            if (s_ConnectorTypeMap.TryGetValue(typeName, out var type))
            {
                return type;
            }

            try
            {
                Assembly asm = Assembly.Load($"SequelNet.Connector.{typeName}");
                type = asm.GetType($"SequelNet.Connector.{typeName}Connector");
                s_ConnectorTypeMap[typeName] = type;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SequelNet.ConnectorBase.GetConnectorType error: {ex}");
            }

            return type;
        }

        /// <summary>
        /// Instantiate a connector from the default type (set through <see cref="SetDefaultConnectorType"/> or <see cref="SetDefaultConnectorTypeByName"/>),
        /// With the specified connection string or the default one (set through <see cref="SetDefaultConnectionString"/>).
        /// 
        /// If you need to use dependency injection, you can <see cref="AddSingleton"/> an <see cref="IConnectorFactory"/> (i.e <see cref="MySql2Factory"/>) set with the connection string.  
        /// Or import `SequelNet.Extensions.Hosting` and call `UseSequelNet`.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ConnectorBase Create(string connectionString = null)
        {
            if (s_DefaultConnectorType == null)
            {
                s_DefaultConnectorType = DetectNetFrameworkConnectorType();
            }
            if (connectionString == null && s_DefaultConnectionString == null)
            {
                s_DefaultConnectionString = DetectNetFrameworkConnectionString(null);
            }

            return (ConnectorBase)Activator.CreateInstance(s_DefaultConnectorType, 
                new string[] { connectionString ?? s_DefaultConnectionString });
        }

        [Obsolete("Use ConnectorBase.Create() or migrate to using an IConnectorFactory")]
        public static ConnectorBase NewInstance()
        {
            return Create(null);
        }

        [Obsolete("Use ConnectorBase.Create(connectionString) and pass a connection string instead of a key, or migrate to using an IConnectorFactory")]
        static public ConnectorBase NewInstance(string connectionStringKey)
        {
            if (s_DefaultConnectorType == null)
            {
                s_DefaultConnectorType = DetectNetFrameworkConnectorType();
            }

            return (ConnectorBase)Activator.CreateInstance(s_DefaultConnectorType, new string[] { DetectNetFrameworkConnectionString(connectionStringKey) });
        }

        virtual public SqlServiceType TYPE
        {
            get { return SqlServiceType.UNKNOWN; }
        }

        private static Type DetectNetFrameworkConnectorType()
        {
            try
            {
                string connector = ConfigurationManager.AppSettings["SequelNet.Connector"];
                if (!string.IsNullOrEmpty(connector))
                {
                    return GetConnectorType(connector);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(@"SequelNet.ConnectorBase.FindConnectorType error: {0}", ex));
            }

            return null;
        }

        private static string DetectNetFrameworkConnectionString(string connectionStringKey)
        {
            ConnectionStringSettings connString = null;

            if (connectionStringKey != null)
            {
                connString = ConfigurationManager.ConnectionStrings[connectionStringKey];
                if (connString != null) return connString.ConnectionString;
            }

            string appConnString = ConfigurationManager.AppSettings["SequelNet::ConnectionStringKey"];
            if (appConnString != null && appConnString.Length > 0) connString = ConfigurationManager.ConnectionStrings[appConnString];
            if (connString != null) return connString.ConnectionString;

            appConnString = ConfigurationManager.AppSettings["SequelNet::ConnectionString"];
            if (appConnString != null && appConnString.Length > 0) return appConnString;

            connString = ConfigurationManager.ConnectionStrings["SequelNet"];
            if (connString != null) return connString.ConnectionString;

            return ConfigurationManager.ConnectionStrings[0].ConnectionString;
        }

        public static void SetDefaultConnectorTypeByName(string typeName)
        {
            s_DefaultConnectorTypeName = typeName;
            s_DefaultConnectorType = typeName == null ? null : GetConnectorType(typeName);
        }

        public static string GetDefaultConnectorTypeByName()
        {
            return s_DefaultConnectorTypeName;
        }

        public static void SetDefaultConnectorType(Type type)
        {
            s_DefaultConnectorTypeName = type?.Name;
            s_DefaultConnectorType = type;
        }

        public static Type GetDefaultConnectorType()
        {
            return s_DefaultConnectorType;
        }

        public static void SetDefaultConnectionString(string connectionString)
        {
            s_DefaultConnectionString = connectionString;
        }

        public static string GetDefaultConnectionString()
        {
            return s_DefaultConnectionString;
        }

        abstract public IConnectorFactory Factory
        {
            get;
        }

        public DbConnection Connection
        {
            get { return _Connection; }
            protected set { _Connection = value; }
        }

        private static LanguageFactory _LanguageFactory = new LanguageFactory();

        public virtual LanguageFactory Language => _LanguageFactory;

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
            if (Connection.State == ConnectionState.Closed) 
                Connection.Open();

            command.Connection = Connection;
            command.Transaction = Transaction;
            return command.ExecuteNonQuery();
        }

        public virtual async Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken? cancellationToken = null)
        {
            if (Connection.State == ConnectionState.Closed)
                await Connection.OpenAsync().ConfigureAwait(false);

            command.Connection = Connection;
            command.Transaction = Transaction;
            return await command.ExecuteNonQueryAsync(cancellationToken ?? CancellationToken.None)
                .ConfigureAwait(false);
        }

        public virtual object ExecuteScalar(DbCommand command)
        {
            if (Connection.State == ConnectionState.Closed) 
                Connection.Open();

            command.Connection = Connection;
            command.Transaction = Transaction;
            return command.ExecuteScalar();
        }

        public virtual async Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken? cancellationToken = null)
        {
            if (Connection.State == ConnectionState.Closed)
                await Connection.OpenAsync().ConfigureAwait(false);

            command.Connection = Connection;
            command.Transaction = Transaction;
            return await command.ExecuteScalarAsync(cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        }

        internal virtual DataReader ExecuteReader(
            DbCommand command,
            bool attachCommandToReader,
            CommandBehavior commandBehavior)
        {
            var attachConnectionToReader = (commandBehavior & CommandBehavior.CloseConnection) != CommandBehavior.Default;

            try
            {
                if (Connection.State == ConnectionState.Closed) 
                    Connection.Open();

                command.Connection = Connection;
                command.Transaction = Transaction;

                return new DataReader(
                    command.ExecuteReader(commandBehavior),
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

        public DataReader ExecuteReader(
            DbCommand command,
            CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            return ExecuteReader(command, false, commandBehavior);
        }

        internal virtual async Task<DataReader> ExecuteReaderAsync(
            DbCommand command,
            bool attachCommandToReader,
            CommandBehavior commandBehavior,
            CancellationToken? cancellationToken)
        {
            var attachConnectionToReader = (commandBehavior & CommandBehavior.CloseConnection) != CommandBehavior.Default;

            try
            {
                if (Connection.State == ConnectionState.Closed) 
                    await Connection.OpenAsync().ConfigureAwait(false);

                command.Connection = Connection;
                command.Transaction = Transaction;

                var baseReader = await command.ExecuteReaderAsync(commandBehavior, cancellationToken ?? CancellationToken.None)
                    .ConfigureAwait(false);

                return new DataReader(
                    baseReader,
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

        public Task<DataReader> ExecuteReaderAsync(
            DbCommand command,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            CancellationToken? cancellationToken = null)
        {
            return ExecuteReaderAsync(command, false, commandBehavior, cancellationToken);
        }

        public Task<DataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(command, false, CommandBehavior.Default, cancellationToken);
        }

        public virtual DataSet ExecuteDataSet(DbCommand command)
        {
            if (Connection.State == ConnectionState.Closed) 
                Connection.Open();

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
            if (Connection.State == ConnectionState.Closed) 
                Connection.Open();

            using (var command = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public virtual async Task<int> ExecuteNonQueryAsync(string querySql, CancellationToken? cancellationToken = null)
        {
            if (Connection.State == ConnectionState.Closed)
                await Connection.OpenAsync().ConfigureAwait(false);

            using (var command = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return await command.ExecuteNonQueryAsync(cancellationToken ?? CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }

        public virtual object ExecuteScalar(string querySql)
        {
            if (Connection.State == ConnectionState.Closed) 
                Connection.Open();

            using (var command = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return command.ExecuteScalar();
            }
        }

        public virtual async Task<object> ExecuteScalarAsync(string querySql, CancellationToken? cancellationToken = null)
        {
            if (Connection.State == ConnectionState.Closed)
                await Connection.OpenAsync().ConfigureAwait(false);

            using (var command = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return await command.ExecuteScalarAsync(cancellationToken ?? CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }

        public virtual DataReader ExecuteReader(string querySql, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            if (Connection.State == ConnectionState.Closed) 
                Connection.Open();

            var command = Factory.NewCommand(querySql, Connection, Transaction);
            return ExecuteReader(command, true, commandBehavior);
        }

        public virtual async Task<DataReader> ExecuteReaderAsync(
            string querySql,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            CancellationToken? cancellationToken = null)
        {
            if (Connection.State == ConnectionState.Closed)
                await Connection.OpenAsync().ConfigureAwait(false);

            var command = Factory.NewCommand(querySql, Connection, Transaction);
            return await ExecuteReaderAsync(
                command, 
                true, 
                commandBehavior | CommandBehavior.CloseConnection, 
                cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<DataReader> ExecuteReaderAsync(
            string querySql,
            CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(querySql, CommandBehavior.Default, cancellationToken);
        }

        public virtual DataSet ExecuteDataSet(string querySql)
        {
            if (Connection.State == ConnectionState.Closed) 
                Connection.Open();

            using (var cmd = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return ExecuteDataSet(cmd);
            }
        }

        public abstract int ExecuteScript(string querySql);

        public abstract Task<int> ExecuteScriptAsync(string querySql, CancellationToken? cancellationToken = null);

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

        virtual public void SetIdentityInsert(string tableName, bool enabled) { }

        abstract public bool CheckIfTableExists(string tableName);

        abstract public Task<bool> CheckIfTableExistsAsync(string tableName);

        /// <summary>
        /// Synonym for Language.EscapeLike(expression)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Safe expression to use inside a "LIKE" express</returns>
        public string EscapeLike(string expression)
        {
            return Language.EscapeLike(expression);
        }

        #endregion

        #region Transactions
        
        private DbTransaction _Transaction = null;
        private Stack<DbTransaction> _Transactions = null;

        public virtual bool BeginTransaction()
        {
            try
            {
                if (_Connection.State == ConnectionState.Closed) 
                    _Connection.Open();

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
                if (_Connection.State == ConnectionState.Closed)
                    _Connection.Open();

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
        /// Tests whether a mutext is in use.
        /// </summary>
        /// <param name="lockName">Unique name for the lock</param>
        /// <param name="owner">The owner of the lock. Partial support in different db servers.</param>
        /// <param name="dbPrincipal">The user that has permissions to the lock.</param>
        /// <returns><value>true</value> if the lock is used, <value>false</value> if it's free of there was an invalid argument.</returns>
        public virtual bool IsLockUsed(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            throw new NotImplementedException(@"GetLock");
        }

        /// <summary>
        /// Tests whether a mutext is free.
        /// </summary>
        /// <param name="lockName">Unique name for the lock</param>
        /// <param name="owner">The owner of the lock. Partial support in different db servers.</param>
        /// <param name="dbPrincipal">The user that has permissions to the lock.</param>
        /// <returns><value>true</value> if the lock is free, <value>false</value> if it's used of there was an invalid argument.</returns>
        public virtual bool IsLockFree(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
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
